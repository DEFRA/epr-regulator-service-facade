namespace EPR.RegulatorService.Facade.API.Middlewares;

public static partial class ExceptionMiddlewareLoggerMessages
{
    [LoggerMessage(LogLevel.Error, Message = "{Message}")]
    public static partial void HandleException(this ILogger logger, string message, Exception ex);
}