using DapperHomeWork.Configuration;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Serilog;
using Serilog.Formatting.Json;

namespace DapperHomeWork;

using Models.Consts;
using Options;

public class Program
{
    public static void Main(string[] args)
    {
        CreateSerilog();

        var logger = LogManager.Setup()
            .LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

        try
        {
            logger.Debug("init main");

            CreateWebApplication(args).Run();
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    private static void CreateSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {NewLine} {Exception}")
            .WriteTo.File(@"D:\Programer\DapperHomeWork\DapperHomeWork\bin\Debug\net7.0\logs\Serilog\txt\log.txt",
                rollingInterval: RollingInterval.Minute)
            .WriteTo.File(new JsonFormatter(),
                @"D:\Programer\DapperHomeWork\DapperHomeWork\bin\Debug\net7.0\logs\Serilog\json\log.json",
                rollingInterval: RollingInterval.Minute)
            .CreateLogger();
    }

    public static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

        builder.Host.UseNLog();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,

                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = true
                };
            });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "DapperHomeWork API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        builder.Services.AddOptions()
            .Configure<ConnectionStringsConfiguration>(builder.Configuration.GetSection("ConnectionStrings"));

        builder.Services.AddRepositories();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DapperHomeWork API");
            });
        }

        app.MapControllers();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
