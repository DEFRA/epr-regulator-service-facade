using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;
using Swashbuckle.AspNetCore.Filters;

namespace EPR.RegulatorService.Facade.API.Filters.Swashbuckle;

public class UpdateTaskStatusRequestExample : IExamplesProvider<UpdateTaskStatusRequestDto>
{
    public UpdateTaskStatusRequestDto GetExamples()
    {
        return new UpdateTaskStatusRequestDto
        {
            Status = RegistrationTaskStatus.Queried,
            Comments = "Business address is incomplete."
        };
    }
}
