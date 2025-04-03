using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Filters.Swashbuckle;

[ExcludeFromCodeCoverage]
public class Headers
{
    [FromHeader]
    public Guid UserId { get; set; }
}

[ExcludeFromCodeCoverage]
public class SwashbuckleHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = nameof(Headers.UserId),
            In = ParameterLocation.Header,
            Description = "User Id",
            Required = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
                Format = "uuid",
                Example = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6")
            }
        });
    }
}
