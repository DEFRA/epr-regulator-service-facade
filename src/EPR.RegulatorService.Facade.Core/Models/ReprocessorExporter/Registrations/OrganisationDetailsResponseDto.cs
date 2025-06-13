namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public class OrganisationDetailsResponseDto
{
    public string OrganisationName { get; set; }
    public string TradingName { get; set; }
    public string OrganisationType { get; set; }
    public string CompaniesHouseNumber { get; set; }
    public string RegisteredAddress { get; set; }
    public List<PersonDetailsResponseDto> Persons { get; set; }
}