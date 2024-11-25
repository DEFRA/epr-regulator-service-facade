using EPR.RegulatorService.Facade.Core.Attributes;
using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Helpers.Validators;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

[ExcludeFromCodeCoverage]
public class RegulatorDecisionCreateRequest
{
    [NotDefault]
    public Guid SubmissionId { get; init; }

    [Required]
    [NotNone]
    public RegistrationSubmissionStatus Status { get; init; }

    public string? Comments { get; init; }

    public Guid OrganisationId { get; init; }

    public Guid? UserId { get; init; }

    public CountryName? CountryName { get; init; }

    public RegistrationSubmissionType? RegistrationSubmissionType { get; init; }

    public string ApplicationReferenceNumber { get; init; }

    public string? TwoDigitYear { get; init; }

    // This is the 6 digit Org ID taken from Acct Management
    public string? OrganisationAccountManagementId { get; init; }

    public DateTime? DecisionDate { get; set; }
     
    // add agency name here from FE
    public string?  AgencyName { get; init; }
    public string? AgencyEmail { get; init; }
    // org name   here from FE
    public string? OrganisationName { get; init; }
    // org ref  here from FE
    public string? OrganisationReference { get; init; }
    // org email here from FE
    public string? OrganisationEmail { get; init; }
    // required for the welsh email templates
    public bool? IsWelsh { get; init; } = false;

}