using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;

public interface IAccountServiceClient
{
    Task<NationDetailsResponseDto> GetNationDetailsById(int nationId);
    Task<List<PersonDetailsResponseDto>> GetPersonDetailsByIds(PersonDetailsRequestDto requestDto);
    Task<string> GetOrganisationNameById(Guid id);
}
