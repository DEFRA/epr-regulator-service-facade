using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter;

namespace EPR.RegulatorService.Facade.Core.Clients.PrnBackendServiceClient;

public interface IPrnBackendServiceClient
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request);
}
