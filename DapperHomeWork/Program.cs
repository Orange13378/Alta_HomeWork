using Dapper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Prometheus;
using Serilog;
using Serilog.Formatting.Json;

namespace DapperHomeWork;

using Middleware;
using Models.Consts;
using Options;
using Configuration;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using Npgsql;
using Extensions;

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
            .WriteTo.File(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "Serilog", "txt", "log.txt"),
                rollingInterval: RollingInterval.Minute)
            .WriteTo.File(new JsonFormatter(),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "Serilog", "json", "log.json"),
                rollingInterval: RollingInterval.Minute)
            .CreateLogger();
    }

    public static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

        builder.Host.UseNLog();

        #region OpenTelemetry

        var meter = new Meter("Telemetry.Example", "1.0.0");

        meter.CreateObservableGauge("users_in_db4", () =>
        {
            using var connection = new NpgsqlConnection(builder.Configuration.GetConnectionString("DbConnection"));
            return connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");
        });

        var activitySource = new ActivitySource("Telemetry.Example");

        var activity = activitySource.StartActivity("UsersCounter");

        var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
        var otel = builder.Services.AddOpenTelemetry();

        otel.ConfigureResource(resource => resource
            .AddService(serviceName: builder.Environment.ApplicationName));

        otel.WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddMeter(meter.Name)
            // Metrics provides by ASP.NET Core in .NET 8
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddPrometheusExporter());

        otel.WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation();
            tracing.AddHttpClientInstrumentation();
            tracing.AddSource(activitySource.Name);
            if (tracingOtlpEndpoint != null)
            {
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint);
                });
            }
            else
            {
                tracing.AddConsoleExporter();
            }
        });

        #endregion

        var server = new MetricServer("localhost", 9184);
        server.Start();

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
        builder.Services.AddBus(builder.Configuration.GetSection("Bus"));

        builder.Services.AddMemoryCache();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "192.168.0.103";
            options.InstanceName = "Redis Server";
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "DapperHomeWork API");
        });

        app.UseExceptionMiddleware();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapPrometheusScrapingEndpoint();

        return app;
    }
}
