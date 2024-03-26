namespace EPR.RegulatorService.Facade.Core.Models.Organisations;

public class AssociatedPersonResults
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string OrganisationId { get; set; }
    
    public string CompanyName { get; set; } = default!;
    
    public int ServiceRoleId { get; set; }
    
    public string TemplateId { get; set; }
    
    public string EmailNotificationType { get; set; }

    public string AccountSignInUrl { get; set; }
}