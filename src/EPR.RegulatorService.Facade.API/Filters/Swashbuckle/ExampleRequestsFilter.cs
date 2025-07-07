using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Enums;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.API.Filters.Swashbuckle
{
    [ExcludeFromCodeCoverage]
    public class ExampleRequestsFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.Name == "GetRegistrationSubmissionList")
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Example = new OpenApiString(JsonConvert.SerializeObject(
                                new
                                {
                                    OrganisationName = "",
                                    OrganisationReference = "",
                                    OrganisationType  = "",
                                    Statuses  = "",
                                    ResubmissionStatuses  = "",
                                    RelevantYears  = "",
                                    NationId = 1,
                                    PageNumber = 1,
                                    PageSize = 20
                                })
                            )
                        }
                    }
                };
            }
            if (context.MethodInfo.Name == nameof(OrganisationRegistrationSubmissionsController.GetRegistrationSubmissionDetails))
            {
                var subIdParam = operation.Parameters.First(p => p.Name == "submissionId");
                subIdParam.Example = new OpenApiString("c8b7d333-ce94-46fb-8855-e335c4b135f2");
                var orgTypeParam = operation.Parameters.First(p => p.Name == "organisationType");
                orgTypeParam.Example = new OpenApiString(OrganisationType.ComplianceScheme.ToString());

                var lateFeeParam = operation.Parameters.First(p => p.Name == "lateFeeRules");
                lateFeeParam.Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties =
                    {
                        ["LateFeeCutOffMonth_2025"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("4") },
                        ["LateFeeCutOffDay_2025"]   = new OpenApiSchema { Type = "string", Example = new OpenApiString("1") },
                        ["LateFeeCutOffMonth_CS"]   = new OpenApiSchema { Type = "string", Example = new OpenApiString("10") },
                        ["LateFeeCutOffDay_CS"]     = new OpenApiSchema { Type = "string", Example = new OpenApiString("1") },
                        ["LateFeeCutOffMonth_SP"]   = new OpenApiSchema { Type = "string", Example = new OpenApiString("4") },
                        ["LateFeeCutOffDay_SP"]     = new OpenApiSchema { Type = "string", Example = new OpenApiString("1") },
                        ["LateFeeCutOffMonth_LP"]   = new OpenApiSchema { Type = "string", Example = new OpenApiString("10") },
                        ["LateFeeCutOffDay_LP"]     = new OpenApiSchema { Type = "string", Example = new OpenApiString("1") }
                    },
                    AdditionalPropertiesAllowed = false,
                    Example = new OpenApiObject
                    {
                        ["LateFeeCutOffMonth_2025"] = new OpenApiString("4"),
                        ["LateFeeCutOffDay_2025"] = new OpenApiString("1"),
                        ["LateFeeCutOffMonth_CS"] = new OpenApiString("10"),
                        ["LateFeeCutOffDay_CS"] = new OpenApiString("1"),
                        ["LateFeeCutOffMonth_SP"] = new OpenApiString("4"),
                        ["LateFeeCutOffDay_SP"] = new OpenApiString("1"),
                        ["LateFeeCutOffMonth_LP"] = new OpenApiString("10"),
                        ["LateFeeCutOffDay_LP"] = new OpenApiString("1")
                    }
                };
                lateFeeParam.Style = ParameterStyle.DeepObject;
                lateFeeParam.Explode = true;
            }
        }
    }
}

