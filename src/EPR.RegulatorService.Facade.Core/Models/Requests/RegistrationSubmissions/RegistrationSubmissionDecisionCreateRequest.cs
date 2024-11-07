using EPR.RegulatorService.Facade.Core.Attributes;
using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegistrationSubmissionDecisionCreateRequest
{
    [NotDefault]
    public Guid SubmissionId { get; set; }

    [Required]
    public RegistrationStatus Status { get; set; }

    public string? Comments { get; set; }

    public Guid OrganisationId { get; set; }

    public Guid? UserId { get; set; }
     
    public CountryName CountryName { get; set; }

    public RegistrationSubmissionType RegistrationSubmissionType { get; set; }

    public string? TwoDigitYear { get; set; } = null;


    // This is the 6 digit Org ID taken from Acct Management
    public string? OrganisationAccountManagementId { get; set; }
}