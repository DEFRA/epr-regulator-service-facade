using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public interface IAccountServiceClient
{
    Task<NationDetailsResponseDto> GetNationDetailsById(int id);
    Task<OrganisationDetailsResponseDto> GetOrganisationDetailsById(Guid id);
}
