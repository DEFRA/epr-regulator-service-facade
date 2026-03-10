using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EPR.RegulatorService.Facade.API.HealthChecks;

[ExcludeFromCodeCoverage]
public static class HealthCheckOptionBuilder
{
    public static HealthCheckOptions Build(string? buildNumber = null, string? gitSha = null) => new()
    {
        AllowCachingResponses = false,
        ResultStatusCodes =
        {
            [HealthStatus.Healthy] = StatusCodes.Status200OK
        },
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                buildNumber = buildNumber ?? "NOT_SET",
                gitSha = gitSha ?? "NOT_SET"
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    };
}

