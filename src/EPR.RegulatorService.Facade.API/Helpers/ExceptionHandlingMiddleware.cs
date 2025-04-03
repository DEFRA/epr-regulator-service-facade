using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
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
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = (int)HttpStatusCode.InternalServerError;
        ActionResult response;

        if (ex is HttpRequestException requestException)
        {
            response = HandleErrorWithStatusCode(requestException.StatusCode);
        }
        else
        {
            response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        switch (response)
        {
            case BadRequestResult:
                statusCode = (int)HttpStatusCode.BadRequest;
                break;
            case NotFoundResult:
                statusCode = (int)HttpStatusCode.NotFound;
                break;
            case ForbidResult:
                statusCode = (int)HttpStatusCode.Forbidden;
                break;
            case StatusCodeResult statusCodeResult:
                statusCode = statusCodeResult.StatusCode;
                break;
            case ObjectResult objectResult:
                statusCode = objectResult.StatusCode ?? (int)HttpStatusCode.InternalServerError;
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        context.Response.StatusCode = statusCode;

        var errorResponse = new
        {
            title = "An error occurred while processing your request",
            status = statusCode,
            detail = ex?.InnerException?.Message
        };

        await context.Response.WriteAsJsonAsync(errorResponse);
    }


    private static ActionResult HandleErrorWithStatusCode(HttpStatusCode? statusCode)
    {
        switch (statusCode)
        {
            case HttpStatusCode.BadRequest:
                return new BadRequestResult();
            case HttpStatusCode.NotFound:
                return new NotFoundResult();
            case HttpStatusCode.Forbidden:
                return new ForbidResult();
            default:
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }
    }
}
