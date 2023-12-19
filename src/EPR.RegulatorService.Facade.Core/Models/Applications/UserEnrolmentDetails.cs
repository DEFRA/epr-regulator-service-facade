using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Applications;

[ExcludeFromCodeCoverage]
public class UserEnrolmentDetails
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string TelephoneNumber { get; set; }

    public string JobTitle { get; set; }

    public bool IsEmployeeOfOrganisation { get; set; }

    public EnrolmentDetails Enrolments { get; set; }
    
    public string? TransferComments { get; set; }
}