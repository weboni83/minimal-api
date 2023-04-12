using AutoMapper;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalAPIsDemo.Data;
using MinimalAPIsDemo.DTOs;
using MinimalAPIsDemo.Entities;
using MinimalAPIsDemo.Messages;
using MinimalAPIsDemo.Policy;
using MinimalAPIsDemo.Profiles;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DbContextClass>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// lower case
builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = LowerCaseNamingPolicy.LowerCase);

// logger config
builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
.ReadFrom.Configuration(ctx.Configuration));

// 환경변수 추가.
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddEnvironmentVariables();
});


var app = builder.Build();

// By default, the ASP.NET Core framework logs multiple information-level events per request.
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var configuration = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new AppProfile());
});
var mapper = configuration.CreateMapper();

var defaultApp = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
});

app.Logger.LogInformation($"InitFirebaseApp => {defaultApp.Name}");

//Request할 때 고정된 값을 사용한다.
const string PLANT_CD = "001";

//get the list of parameter
app.MapGet("/users", async (DbContextClass dbContext) =>
{
    var users = await dbContext.SYS_USER_INFOs.ToListAsync();

    if (users == null)
    {
        return Results.NoContent();
    }

    var results = mapper.Map<List<SYS_USER_INFO>, List<UserDTO>>(users);

    return Results.Ok(results);
});

//get user by id
app.MapGet("/user/{id}", async ([FromRoute] string id, DbContextClass dbContext) =>
{
    var user = await dbContext.SYS_USER_INFOs.FindAsync(PLANT_CD, id);
    if (user == null)
    {
        return Results.NotFound();
    }

    var result = mapper.Map<SYS_USER_INFO, UserDTO>(user);

    return Results.Ok(result);
});

//get alarm by by user_id
app.MapGet("/user/{id}/alarm", async ([FromRoute] string id, string read_yn, DbContextClass dbContext) =>
{
    const string FORM_COMMON_CD = "ELECTRO_SIGN_GB";
    var alarms = await dbContext.SYS_ALARMs.Where(p => p.PLANT_CD == PLANT_CD
    && p.RECEIVE_EMP_CD == id
    && p.READ_YN == read_yn
    ).ToListAsync();
    if (alarms == null || alarms.Count == 0)
    {
        var message = new ResponseDTO() { IsSuccess = false, Message = Response.NOT_FOUND };
        return Results.NotFound(message);
    }

    var commons = await dbContext.CM_COMMONs.Where(p => p.PLANT_CD == PLANT_CD
    && p.COMMON_CD == FORM_COMMON_CD).ToListAsync();

    //alarms.ForEach(p => {
    //    p.FORM_NAME = commons.Where(c => c.COMMON_PART_CD == p.FORM_NAME)
    //    .FirstOrDefault().COMMON_PART_NM;
    //});
    //var results = mapper.Map<List<SYS_ALARM>, List<AlarmDTO>>(alarms);

    // instead of above code 

    // using lambda expression
    var results = alarms.GroupJoin(commons,
                         alarm => alarm.FORM_NAME,
                         common => common.COMMON_PART_CD,
                         (alarm, common) => new { alarm, common })
    .SelectMany(
    item => item.common.DefaultIfEmpty()
    , (code, name) => 
    new AlarmDTO
    { 
        PLANT_CD = code.alarm.PLANT_CD
        , ALARM_ID = code.alarm.ALARM_ID
        // common_part_cd 가 없을 경우 null 반환
        , ALARM_TITLE = name?.COMMON_PART_NM
        , ALARM_CONTENT = code.alarm.ALARM_CONTENT
        , RECEIVE_EMP_CD = code.alarm.RECEIVE_EMP_CD
        , READ_YN = code.alarm.READ_YN
        , READ_TIME = code.alarm.READ_TIME
    });

    return Results.Ok(results);
});

app.MapPut("/user/{id}/update", async ([FromRoute] string id, UserTokenDTO userToken, DbContextClass dbContext) =>
{
    var user = await dbContext.SYS_USER_INFOs.FindAsync(PLANT_CD, id);
    if (user == null)
    {
        return Results.NotFound();
    }
    user.DEVICE_TOKEN = userToken.DEVICE_TOKEN;

    await dbContext.SaveChangesAsync();

    var result = mapper.Map<SYS_USER_INFO, UserDTO>(user);

    return Results.Ok(result);
});

// Post Notification
app.MapPost("/user/{id}/send", async ([FromRoute] string id, NotificationUserDTO notificationUserDTO, DbContextClass dbContext) =>
{
    var user_id = id;
    var user = await dbContext.SYS_USER_INFOs.FindAsync(PLANT_CD, user_id);
    if (user == null)
    {
        var resturnMessage = new ResponseDTO() { IsSuccess = false, Message = Response.NOT_FOUND };
        return Results.NotFound(resturnMessage);
    }

    if (string.IsNullOrEmpty(user.DEVICE_TOKEN))
    {
        var resturnMessage = new ResponseDTO() { IsSuccess = false, Message = Response.NOT_FOUND_TOKEN };
        return Results.NotFound(resturnMessage);
    }

    var token = user.DEVICE_TOKEN;
    // Client의 토큰
    var messaging = FirebaseMessaging.DefaultInstance;
    var message = new Message()
    {
        Token = token,
        Data = notificationUserDTO.Data,
    };

    var result = await messaging.SendAsync(message);

    if (result == null)
    {
        return Results.NoContent();
    }

    return Results.Ok(result);
});

//update the alarm by alarm_id
app.MapPut("/alarm/{id}/update", async ([FromRoute] long id, AlarmUpdateDTO alarmDTO, DbContextClass dbContext) =>
{
   var alarmDetail = await dbContext.SYS_ALARMs.FindAsync(PLANT_CD, id);
    if (alarmDetail == null)
    {
        var message = new ResponseDTO() { IsSuccess = false, Message = Response.NOT_FOUND };
        return Results.NotFound(message);
    }

    if (alarmDTO == null || alarmDTO.READ_YN.Length == 0)
    {
        var message = new ResponseDTO() { IsSuccess = false, Message = Response.NOT_FOUND_UPDATE };
        return Results.NotFound(message);
    }

    alarmDetail.READ_YN = alarmDTO.READ_YN.ToUpper();

    if (alarmDetail.READ_YN == "Y")
        alarmDetail.READ_TIME = DateTime.Now;

    var result = mapper.Map<SYS_ALARM, AlarmDTO>(alarmDetail);

    await dbContext.SaveChangesAsync();
    return Results.Ok(result);
});




// Post Notification
app.MapPost("/notification/device/{token}/send", async ([FromRoute] string token, NotificationDTO notificationDTO, DbContextClass dbContext) =>
{
    // DeviceId에 토큰을 실어서 테스트함.
    //var token = notificationDTO.DeviceId;
    // Client의 토큰
    var messaging = FirebaseMessaging.DefaultInstance;
    var message = new Message() { 
        Token = token,
        Data = notificationDTO.Data,
    };
    
    var result = await messaging.SendAsync(message);

    if (result == null)
    {
        return Results.NoContent();
    }

    return Results.Ok(result);
});

app.Run();
