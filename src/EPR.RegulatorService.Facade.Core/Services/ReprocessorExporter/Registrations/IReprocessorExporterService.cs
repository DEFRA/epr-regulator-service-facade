using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public interface IReprocessorExporterService
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request);
    Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(Guid id);
    Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(Guid id);
    Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(Guid id, UpdateMaterialOutcomeRequestDto request);
    Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(Guid id);
    Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(Guid id);
    Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(Guid id);
    Task<SiteAddressDetailsDto> GetSiteAddressByRegistrationId(Guid id);
    Task<RegistrationWasteCarrierDto> GetWasteCarrierDetailsByRegistrationId(Guid id);
    Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(Guid id);
    Task<PaymentFeeDetailsDto> GetPaymentFeeDetailsByRegistrationMaterialId(Guid id);
    Task<bool> SaveOfflinePayment(Guid userId, OfflinePaymentRequestDto request);
    Task<bool> MarkAsDulyMadeByRegistrationMaterialId(Guid id, Guid userId, MarkAsDulyMadeRequestDto request);
    Task<RegistrationOverviewDto> GetRegistrationByIdWithAccreditationsAsync(Guid id, int? year);
    Task<AccreditationSamplingPlanDto> GetSamplingPlanByAccreditationId(Guid id);
    Task<AccreditationPaymentFeeDetailsDto> GetAccreditationPaymentFeeDetailsByAccreditationId(Guid id);
    Task<bool> MarkAsDulyMadeByAccreditationId(Guid id, Guid userId, MarkAsDulyMadeRequestDto request);
    Task<bool> UpdateRegulatorAccreditationTaskStatus(Guid userId, UpdateAccreditationTaskStatusDto request);
    Task<bool> SaveAccreditationOfflinePayment(Guid userId, OfflinePaymentRequestDto request);
    Task<bool> SaveApplicationTaskQueryNotes(Guid id, Guid userId, QueryNoteRequestDto request);
    Task<bool> SaveRegistrationTaskQueryNotes(Guid id, Guid userId, QueryNoteRequestDto request);
}
