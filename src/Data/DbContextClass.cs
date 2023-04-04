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
        private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<DbContextClass>();

        protected readonly IConfiguration Configuration;
        public DbContextClass(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var source = Configuration["DataSource"];
            var db = Configuration["Catalog"];
            var user= Configuration["UserId"];
            var password = Configuration["Password"];

            var conStrBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("MimimalAPI"));
            conStrBuilder.DataSource = source;
            conStrBuilder.InitialCatalog = db;
            conStrBuilder.UserID = user;
            conStrBuilder.Password = password;

            var connectionString = conStrBuilder.ConnectionString;
            // instead of above code
            //string connectionString = Configuration.GetConnectionString("MimimalAPI");
            options.UseSqlServer(connectionString);
            //options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSerilog()));
            options.EnableSensitiveDataLogging().
            LogTo(log => 
             {
                 if (log.Contains("CommandExecuted"))
                 {
                     Log.Debug(log);
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
            .Entity<SYS_PARAMETER>()
            .HasKey(c => new { c.PLANT_CD, c.PARAMETER_CD });

            modelBuilder
            .Entity<SYS_USER_INFO>()
            .HasKey(c => new { c.PLANT_CD, c.USER_ID });

            modelBuilder
            .Entity<SYS_ALARM>()
            .HasKey(c => new { c.PLANT_CD, c.ALARM_ID });
        }

        public DbSet<CM_PLANT> CM_PLANTs { get; set; }
        public DbSet<CM_COMMON> CM_COMMONs { get; set; }
        public DbSet<SYS_PARAMETER> SYS_PARAMETERs { get; set; }
        public DbSet<SYS_USER_INFO> SYS_USER_INFOs { get; set; }
        public DbSet<SYS_ALARM> SYS_ALARMs { get; set; }
    }
}
