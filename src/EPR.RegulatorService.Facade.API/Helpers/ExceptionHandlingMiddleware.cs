using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Middleware;

[ExcludeFromCodeCoverage]
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (HttpRequestException httpRequestException)
        {
            logger.LogError(httpRequestException, "An HTTP request exception occurred.");
            await HandleHttpRequestExceptionAsync(httpContext, httpRequestException);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleHttpRequestExceptionAsync(HttpContext context, HttpRequestException ex)
    {
        var statusCode = (int)(ex.StatusCode ?? HttpStatusCode.InternalServerError);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            title = "An HTTP request error occurred.",
            status = statusCode,
            detail = ex.InnerException?.Message
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            HttpRequestException httpRequestException => (int)(httpRequestException.StatusCode ?? HttpStatusCode.InternalServerError),
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            title = "An error occurred while processing your request",
            status = statusCode,
            detail = ex?.InnerException?.Message
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}
