using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            HttpRequestException httpRequestException => (int)(httpRequestException.StatusCode ?? HttpStatusCode.InternalServerError),
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            ValidationException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        if (ex is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var errorResponse = new
            {
                status = StatusCodes.Status400BadRequest,
                title = "One or more validation errors occurred.",
                detail = validationException.InnerException?.Message,
                errors
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
        else
        {
            var errorResponse = new
            {
                title = ex is HttpRequestException ? "An HTTP request error occurred." : "An error occurred while processing your request",
                status = statusCode,
                detail = ex?.InnerException?.Message
            };

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
