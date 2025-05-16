using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;

public interface IRegistrationServiceClient
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request);
    Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id);
    Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id);
    Task<RegistrationAccreditationReferenceDto> GetRegistrationAccreditationReference(int id);
    Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeWithReferenceDto request);
    Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(int id);
    Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(int id);
    Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(int id);
    Task<RegistrationSiteAddressDto> GetSiteAddressByRegistrationId(int id); 
    Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(int id);
    Task<RegistrationFeeContextDto> GetRegistrationFeeRequestByRegistrationMaterialId(int id);
    Task<bool> MarkAsDulyMadeByRegistrationMaterialId(int id, MarkAsDulyMadeWithUserIdDto request);
}
