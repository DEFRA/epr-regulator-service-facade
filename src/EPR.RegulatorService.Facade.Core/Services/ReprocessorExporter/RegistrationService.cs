using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter;

public class RegistrationService(IRegistrationServiceClient client) : IRegistrationService
{
    private readonly IRegistrationServiceClient _client = client;

    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await _client.UpdateRegulatorRegistrationTaskStatus(id, request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await _client.UpdateRegulatorApplicationTaskStatus(id, request);
    }
}