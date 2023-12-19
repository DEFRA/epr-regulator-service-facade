namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

public class OrganisationDetailsUser
{
    public bool? IsEmployee { get; set; }
    public string? JobTitle { get; set; }
    public string? PhoneNumber { get; set; }
    public int? PersonRoleId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Guid ExternalId { get; set; }
    public List<OrganisationDetailUserEnrolment> UserEnrolments { get; set; }
}