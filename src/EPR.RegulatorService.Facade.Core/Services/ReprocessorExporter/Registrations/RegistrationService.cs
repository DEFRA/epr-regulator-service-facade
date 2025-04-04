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

public class RegistrationService(IRegistrationServiceClient client) : IRegistrationService
{
    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await client.UpdateRegulatorRegistrationTaskStatus(id, request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request)
    {
        return await client.UpdateRegulatorApplicationTaskStatus(id, request);
    }
}