using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using MinimalAPIsDemo.Entities;
using Serilog;
using System.Data.Common;

namespace MinimalAPIsDemo.Data
{
    public class DbContextClass : DbContext
    {
        private static readonly Serilog.ILogger Logger = Serilog.Log.ForContext<DbContextClass>();

        protected readonly IConfiguration Configuration;
        public DbContextClass(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // NOTE: #1. secrets.json 에서 설정값을 읽어오는 방법
            //var conStrBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("MinimalAPI"));
            //conStrBuilder.DataSource = Configuration["DataSource"];
            //conStrBuilder.InitialCatalog = Configuration["Catalog"];
            //conStrBuilder.UserID = Configuration["UserId"];
            //conStrBuilder.Password = Configuration["Password"];

            //var connectionString = conStrBuilder.ConnectionString;

            // NOTE: #2. 환경변수에서 설정값을 읽어오는 방법
            //string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:MinimalAPIConnection");
            string connectionString = Configuration["ConnectionStrings:MinimalAPIConnection"];

            string connectionText = string.Empty;
            if (!string.IsNullOrEmpty(connectionString)) 
            { 
                connectionText += string.Join(";", connectionString.Split(";").Take(2));
                Logger.Information($"Load EnvironmentVariable =>  {connectionText}");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = Configuration.GetConnectionString("MinimalAPI");
                Logger.Information($"Load appsettings.json =>  {connectionString}");
            }

            // instead of above code
            //string connectionString = Configuration.GetConnectionString("MinimalAPI");
            options.UseSqlServer(connectionString);
            //options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSerilog()));
            options.EnableSensitiveDataLogging().
            LogTo(log => 
             {
                 if (log.Contains("CommandExecuted"))
                 {
                     Logger.Debug(log);
                 }
             });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
            .Entity<CM_PLANT>()
            .HasKey(c => c.PLANT_CD);

            modelBuilder
            .Entity<CM_COMMON>()
            .HasKey(c => new { c.PLANT_CD, c.COMMON_CD, c.COMMON_PART_CD });

            modelBuilder
            .Entity<SYS_USER_INFO>()
            .HasKey(c => new { c.PLANT_CD, c.USER_ID });

            modelBuilder
            .Entity<SYS_ALARM>()
            .HasKey(c => new { c.PLANT_CD, c.ALARM_ID });
        }

        public DbSet<CM_PLANT> CM_PLANTs { get; set; }
        public DbSet<CM_COMMON> CM_COMMONs { get; set; }
        public DbSet<SYS_USER_INFO> SYS_USER_INFOs { get; set; }
        public DbSet<SYS_ALARM> SYS_ALARMs { get; set; }
    }
}
