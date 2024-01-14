using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Security.Authentication;
using SharpLogContext;
using Serilog;

namespace DapperHomeWork.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private static readonly DefaultContractResolver _camelCaseResolver = new()
    {
        NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true }
    };

    private static readonly JsonSerializerSettings _camelCaseSettings = new()
    {
        ContractResolver = _camelCaseResolver
    };

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }

        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)GetResponseError(exception);

        var result = exception.Message;

        context.Response.ContentType = "application/json";

        LogContext.Current.AttachValue("exception", exception);

        var value = LogContext.Current.GetValue("exception");

        Log.Information(value.ToString());

        var jsonResult = JsonConvert.SerializeObject(result, _camelCaseSettings);

        return context.Response.WriteAsync(jsonResult);
    }

    

    private static HttpStatusCode GetResponseError(Exception exception)
    {
        return exception switch
        {
            AuthenticationException _ => HttpStatusCode.Unauthorized,
            ArgumentNullException _ => HttpStatusCode.NotFound,
            InvalidOperationException => HttpStatusCode.BadRequest,
            NullReferenceException => HttpStatusCode.NotFound,

            _ => HttpStatusCode.InternalServerError,
        };
    }
}

public static class ExceptionRequestMiddleware
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}

