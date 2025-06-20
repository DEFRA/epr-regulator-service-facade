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
            await HandleExceptionAsync(httpContext, ex, "An invalid operation occurred.");
        }
        catch (HttpRequestException ex)
        {
            await HandleExceptionAsync(httpContext, ex, "An HTTP request exception occurred.");
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(httpContext, ex, "The requested resource could not be found.");
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

    private async Task HandleExceptionAsync(HttpContext context, Exception ex, string title)
    {
        var statusCode = GetStatusCode(ex);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            status = statusCode,
            title = title,
            detail = ex.Message
        };

        logger.LogError(ex, title);
        await context.Response.WriteAsJsonAsync(errorResponse);
    }

    private static int GetStatusCode(Exception ex) =>
        ex switch
        {
            HttpRequestException httpRequestException => (int)(httpRequestException.StatusCode ?? HttpStatusCode.InternalServerError),
            KeyNotFoundException => (int)HttpStatusCode.NotFound,
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            ArgumentException => (int)HttpStatusCode.BadRequest,
            ValidationException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
}
