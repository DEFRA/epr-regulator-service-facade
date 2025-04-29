using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public interface IRegistrationService
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request);
    Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id);
    Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id);
    Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeRequestDto request);
    Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(int id);
    Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(int id);
    Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(int id);
}