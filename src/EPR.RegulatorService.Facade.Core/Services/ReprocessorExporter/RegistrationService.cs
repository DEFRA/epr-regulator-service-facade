using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Clients.PrnBackendServiceClient;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter;

public class RegistrationService(IPrnBackendServiceClient client) : IRegistrationService
{
    private readonly IPrnBackendServiceClient _client = client;

    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await _client.UpdateRegulatorApplicationTaskStatus(id, request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await _client.UpdateRegulatorApplicationTaskStatus(id, request);
    }
}