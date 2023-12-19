using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// ReSharper disable once ClassNeverInstantiated.Global
namespace EPR.RegulatorService.Facade.API.Swagger;

[ExcludeFromCodeCoverage]
public class AddAuthHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(AllowAnonymousAttribute)))
        {
            return;
        }

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = new List<string>()
        });
    }
}