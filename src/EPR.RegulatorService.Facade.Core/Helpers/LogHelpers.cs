using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace EPR.RegulatorService.Facade.Core.Helpers
{
    public static class LogHelpers
    {
        public static void Log(ILogger logger, string data, LogLevel logLevel, [Optional]Exception e)
        {
            if (data != null)
            {
                data = data.Replace('\n', '_').Replace('\r', '_');

                if (logLevel == LogLevel.Information)
                {
                    logger.LogInformation(data);
                }
                if (logLevel == LogLevel.Error) 
                { 
                    logger.LogError(data, e);
                }
            }
        }
    }
}