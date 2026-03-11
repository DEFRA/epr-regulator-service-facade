using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EPR.RegulatorService.Facade.API.Swagger;

[ExcludeFromCodeCoverage]
public class HealthEndpointDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var healthPath = new OpenApiPathItem();
        healthPath.AddOperation(OperationType.Get, new OpenApiOperation
        {
            Tags = new List<OpenApiTag> { new() { Name = "Health" } },
            Summary = "Health check endpoint",
            Description = $"Returns health status with build info.",
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Description = "Healthy",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new()
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["status"] = new() { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("Healthy") },
                                    ["buildNumber"] = new() { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("20201231.9") },
                                    ["gitSha"] = new() { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("3e8b81e790dd423dc65d16b9235f5c3cc6f754e7") },
                                },
                            },
                        },
                    },
                },
            },
        });

        swaggerDoc.Paths.Add("/admin/health", healthPath);
    }
}
