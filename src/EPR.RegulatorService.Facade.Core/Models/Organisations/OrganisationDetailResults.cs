namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

public class OrganisationDetailResults
{
    public OrganisationDetails Company { get; set; }
    public IEnumerable<OrganisationDetailsUser> CompanyUserInformation {get; set;}
}