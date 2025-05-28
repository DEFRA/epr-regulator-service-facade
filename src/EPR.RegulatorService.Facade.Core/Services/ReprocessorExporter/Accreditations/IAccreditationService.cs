using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Accreditations;

public interface IAccreditationService
{ Task<AccreditationPaymentFeeDetailsDto> GetPaymentFeeDetailsByAccreditationMaterialId(Guid id);
    Task<bool> MarkAccreditationMaterialStatusAsDulyMade(Guid userId, AccreditationMarkAsDulyMadeRequestDto request);
    Task<bool> UpdateAccreditationMaterialTaskStatus(Guid userId, UpdateAccreditationMaterialTaskStatusDto request);
}
