namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

public class OrganisationUserOverviewResponseModel
{
    public Guid PersonExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}