using System.Diagnostics.CodeAnalysis;
using System.Net;
using FluentValidation;

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
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(httpContext, ex);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, "An invalid operation occurred.");
        }
        catch (HttpRequestException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "An HTTP request exception occurred.");
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound, "The requested resource could not be found.");
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            status = (int)HttpStatusCode.BadRequest,
            title = "One or more validation errors occurred.",
            detail = ex.Message,
            errors = ex.Errors?.GroupBy(e => e.PropertyName)
                              .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
        };

        logger.LogError(ex, "A validation exception occurred.");
        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode statusCode, string title)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            status = (int)statusCode,
            title = title,
            detail = ex.Message
        };

        logger.LogError(ex, title);
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
}
