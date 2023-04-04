using AutoMapper;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
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

var app = builder.Build();

// By default, the ASP.NET Core framework logs multiple information-level events per request.
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
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
Console.WriteLine(defaultApp.Name);


//get the list of parameter
app.MapGet("/userlist", async (DbContextClass dbContext) =>
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
app.MapGet("/getuserbyid", async (string plant_cd, string user_Id, DbContextClass dbContext) =>
{
    var user = await dbContext.SYS_USER_INFOs.FindAsync(plant_cd, user_Id);
    if (user == null)
    {
        return Results.NotFound();
    }

    var result = mapper.Map<SYS_USER_INFO, UserDTO>(user);

    return Results.Ok(result);
});

//get alarm by by user_id
app.MapGet("/getalarmbyuserid", async (string plant_cd, string user_Id, DbContextClass dbContext) =>
{
    const string FORM_COMMON_CD = "ELECTRO_SIGN_GB";
    var alarms = await dbContext.SYS_ALARMs.Where(p => p.PLANT_CD == plant_cd
    && p.RECEIVE_EMP_CD == user_Id).ToListAsync();
    if (alarms == null || alarms.Count == 0)
    {
        var message = new ResponseDTO() { IsSuccess = false, Message = Response.NOT_FOUND };
        return Results.NotFound(message);
    }

    var commons = await dbContext.CM_COMMONs.Where(p => p.PLANT_CD == plant_cd
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

//update the alarm by alarm_id
app.MapPut("/updatealarm_id", async (AlarmDTO alarmDTO, DbContextClass dbContext) =>
{
   var alarmDetail = await dbContext.SYS_ALARMs.FindAsync(alarmDTO.PLANT_CD, alarmDTO.ALARM_ID);
    if (alarmDetail == null)
    {
        var message = new ResponseDTO() { IsSuccess = false, Message = Response.NOT_FOUND };
        return Results.NotFound(message);
    }

    if (alarmDTO.ALARM_CONTENT is not null)
        alarmDetail.ALARM_CONTENT = alarmDTO.ALARM_CONTENT;

    alarmDetail.READ_YN = alarmDTO.READ_YN;

    if (alarmDTO.READ_YN == "Y")
        alarmDetail.READ_TIME = DateTime.Now;

    var result = mapper.Map<SYS_ALARM, AlarmDTO>(alarmDetail);

    await dbContext.SaveChangesAsync();
    return Results.Ok(result);
});

// Post Notification
app.MapPost("/notification/send", async (NotificationDTO notificationDTO, DbContextClass dbContext) =>
{
    // DeviceId에 토큰을 실어서 테스트함.
    var token = notificationDTO.DeviceId;
    // Client의 토큰
    var messaging = FirebaseMessaging.DefaultInstance;
    var message = new Message() { 
        Token = token,
        //Topic = topic,
        Data = notificationDTO.Data,
        //Notification = new Notification
        //{
        //    Title = notificationDTO.Title,
        //    Body = notificationDTO.Body
        //},
        //Android = new AndroidConfig()
        //{
        //    TimeToLive = TimeSpan.FromHours(1),
        //    Notification = new AndroidNotification()
        //    {
        //        ClickAction = "TOP_STORY_ACTIVITY",
        //        Icon = "stock_ticker_update",
        //        Color = "#f45342",
        //    },
        //},
    };
    
    var result = await messaging.SendAsync(message);

    if (result == null)
    {
        return Results.NoContent();
    }

    return Results.Ok(result);
});



// Example
//get the list of parameter
app.MapGet("/parameterlist", async (DbContextClass dbContext) =>
{
    var parameter = await dbContext.SYS_PARAMETERs.ToListAsync();
    if (parameter == null)
    {
        return Results.NoContent();
    }
    return Results.Ok(parameter);
});

//get parameter by id
app.MapGet("/getparameterbyid", async (int id, DbContextClass dbContext) =>
{
    var parameter = await dbContext.SYS_PARAMETERs.FindAsync(id);
    if (parameter == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(parameter);
});

//create a new parameter
app.MapPost("/createparameter", async (SYS_PARAMETER parameter, DbContextClass dbContext) =>
{
    var result = dbContext.SYS_PARAMETERs.Add(parameter);
    await dbContext.SaveChangesAsync();
    return Results.Ok(result.Entity);
});

//update the parameter
app.MapPut("/updateparameter", async (SYS_PARAMETER parameter, DbContextClass dbContext) =>
{
    var parameterDetail = await dbContext.SYS_PARAMETERs.FindAsync(parameter.PARAMETER_CD);
    if (parameter == null)
    {
        return Results.NotFound();
    }
    parameterDetail.PARAMETER_VALUE = parameter.PARAMETER_CD;
    parameterDetail.PARAMETER_REMARK = parameter.PARAMETER_REMARK;

    await dbContext.SaveChangesAsync();
    return Results.Ok(parameterDetail);
});

//delete the parameter by id
app.MapDelete("/deleteparameter/{id}", async (int id, DbContextClass dbContext) =>
{
    var parameter = await dbContext.SYS_PARAMETERs.FindAsync(id);
    if (parameter == null)
    {
        return Results.NoContent();
    }
    dbContext.SYS_PARAMETERs.Remove(parameter);
    await dbContext.SaveChangesAsync();
    return Results.Ok();
});

app.Run();
