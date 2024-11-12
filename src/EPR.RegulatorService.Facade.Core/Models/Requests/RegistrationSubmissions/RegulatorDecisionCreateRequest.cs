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

    public CountryName CountryName { get; init; }

    public RegistrationSubmissionType RegistrationSubmissionType { get; init; }

    public string? TwoDigitYear { get; init; }
    
    // This is the 6 digit Org ID taken from Acct Management
    public string? OrganisationAccountManagementId { get; init; }
}