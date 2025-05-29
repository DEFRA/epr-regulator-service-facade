using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using System.Threading.Tasks;

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
    Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(Guid id); 
    Task<PaymentFeeDetailsDto> GetPaymentFeeDetailsByRegistrationMaterialId(Guid id);
    Task<bool> SaveOfflinePayment(Guid userId, OfflinePaymentRequestDto request);
    Task<bool> MarkAsDulyMadeByRegistrationMaterialId(Guid id, Guid userId, MarkAsDulyMadeRequestDto request);
    Task<RegistrationOverviewDto> GetRegistrationByIdWithAccreditationsAsync(Guid id, int? year);
    Task<bool> SaveApplicationTaskQueryNotes(Guid id, Guid userId, QueryNoteRequestDto request);
    Task<bool> SaveRegistrationTaskQueryNotes(Guid id, Guid userId, QueryNoteRequestDto request);
}
