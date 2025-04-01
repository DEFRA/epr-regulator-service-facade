using System.Threading.Tasks;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;

public interface IRegistrationService
{
    Task<bool> UpdateRegulatorRegistrationTaskStatus(int id, UpdateTaskStatusRequestDto request);
    Task<bool> UpdateRegulatorApplicationTaskStatus(int id, UpdateTaskStatusRequestDto request);
}