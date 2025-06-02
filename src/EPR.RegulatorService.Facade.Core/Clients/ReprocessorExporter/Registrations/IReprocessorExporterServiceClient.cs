using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;

public interface IReprocessorExporterServiceClient
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request);
    Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(Guid id);
    Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(Guid id);
    Task<RegistrationAccreditationReferenceDto> GetRegistrationAccreditationReference(Guid id);
    Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(Guid id, UpdateMaterialOutcomeWithReferenceDto request);
    Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(Guid id);
    Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(Guid id);
    Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(Guid id);
    Task<RegistrationSiteAddressDto> GetSiteAddressByRegistrationId(Guid id); 
    Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(Guid id);
    Task<RegistrationFeeContextDto> GetRegistrationFeeRequestByRegistrationMaterialId(Guid id);
    Task<bool> MarkAsDulyMadeByRegistrationMaterialId(Guid id, MarkAsDulyMadeWithUserIdDto request);
    Task<RegistrationOverviewDto> GetRegistrationByIdWithAccreditationsAsync(Guid id, int? year);
    Task<AccreditationSamplingPlanDto> GetSamplingPlanByAccreditationId(Guid id);
    Task<bool> SaveApplicationTaskQueryNotes(Guid id, QueryNoteRequestDto request);
    Task<bool> SaveRegistrationTaskQueryNotes(Guid id, QueryNoteRequestDto request);
}
