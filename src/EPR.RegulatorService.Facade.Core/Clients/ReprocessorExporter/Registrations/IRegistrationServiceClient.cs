using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;

public interface IRegistrationServiceClient
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request);
    Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id);
    Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id);
    Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeRequestDto request);
}
