using System.Net;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Middlewares;

[ExcludeFromCodeCoverage]
public class CustomExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlingMiddleware> logger)
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
        var statusCode = GetStatusCode(ex);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = ex switch
        {
            ValidationException validationException => CreateValidationErrorResponse(validationException),
            HttpRequestException => CreateHttpRequestErrorResponse(ex),
            _ => CreateGenericErrorResponse(ex, statusCode)
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private static int GetStatusCode(Exception ex) =>
        ex switch
        {
            HttpRequestException httpRequestException => (int)(httpRequestException.StatusCode ?? HttpStatusCode.InternalServerError),
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            ValidationException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

    private static object CreateValidationErrorResponse(ValidationException validationException)
    {
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new
        {
            status = StatusCodes.Status400BadRequest,
            title = "One or more validation errors occurred.",
            detail = validationException.Message,
            errors
        };
    }

    private static object CreateHttpRequestErrorResponse(Exception ex) =>
        new
        {
            title = "An HTTP request error occurred.",
            status = (int)HttpStatusCode.InternalServerError,
            detail = ex?.Message
        };

    private static object CreateGenericErrorResponse(Exception ex, int statusCode) =>
        new
        {
            title = "An error occurred while processing your request.",
            status = statusCode,
            detail = ex?.Message
        };
}