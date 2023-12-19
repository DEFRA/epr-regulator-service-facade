namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

public class OrganisationUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<OrganisationUserEnrolment> Enrolments { get; set; }
}
