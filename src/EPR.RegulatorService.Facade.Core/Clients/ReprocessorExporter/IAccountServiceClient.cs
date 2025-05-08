using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public interface IAccountServiceClient
{
    Task<string> GetNationNameById(int id);
    Task<string> GetOrganisationNameById(int id);
}
