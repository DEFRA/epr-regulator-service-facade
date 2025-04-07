using System.Threading.Tasks;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public class RegistrationService(IRegistrationServiceClient registrationServiceClient) : IRegistrationService
{
    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await registrationServiceClient.UpdateRegulatorRegistrationTaskStatus(id, request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await registrationServiceClient.UpdateRegulatorApplicationTaskStatus(id, request);
    }

    public async Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id)
    {
        return await registrationServiceClient.GetRegistrationByRegistrationId(id);
    }

    public async Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id)
    {
        return await registrationServiceClient.GetRegistrationMaterialByRegistrationMaterialId(id);
    }

    public async Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeRequestDto request)
    {
        return await registrationServiceClient.UpdateMaterialOutcomeByRegistrationMaterialId(id, request);
    }
}
