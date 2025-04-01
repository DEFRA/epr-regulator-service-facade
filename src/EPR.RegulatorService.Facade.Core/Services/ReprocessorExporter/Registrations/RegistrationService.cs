using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public class RegistrationService(IRegistrationServiceClient registrationServiceClient) : IRegistrationService
{
    private readonly IRegistrationServiceClient _registrationServiceClient = registrationServiceClient;

    public async Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id)
    {
        return await _registrationServiceClient.GetRegistrationByRegistrationId(id);
    }

    public async Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id)
    {
        return await _registrationServiceClient.GetRegistrationMaterialByRegistrationMaterialId(id);
    }
    public async Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeRequestDto request)
    {
        return await _registrationServiceClient.UpdateMaterialOutcomeByRegistrationMaterialId(id, request);
    }
}
