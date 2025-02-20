using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EPR.RegulatorService.Facade.API.Filters.Swashbuckle
{
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
        }
    }
}

