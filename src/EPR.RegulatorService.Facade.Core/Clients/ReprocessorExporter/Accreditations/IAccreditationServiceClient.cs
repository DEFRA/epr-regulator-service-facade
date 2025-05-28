using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Accreditations;

public interface IAccreditationServiceClient
{
    Task<AccreditationFeeContextDto> GetAccreditationFeeRequestByRegistrationMaterialId(Guid id);
    Task<bool> MarkAccreditationMaterialStatusAsDulyMade(AccreditationMarkAsDulyMadeWithUserIdDto request);
    Task<bool> UpdateAccreditationMaterialTaskStatus(UpdateAccreditationMaterialTaskStatusWithUserIdDto request);
}
