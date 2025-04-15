using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Enums.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using Swashbuckle.AspNetCore.Filters;

namespace EPR.RegulatorService.Facade.API.Filters.Swashbuckle;

[ExcludeFromCodeCoverage]
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
