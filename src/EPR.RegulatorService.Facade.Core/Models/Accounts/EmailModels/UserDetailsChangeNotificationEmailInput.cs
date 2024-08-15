using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

[ExcludeFromCodeCoverage]
public class UserDetailsChangeNotificationEmailInput
{
    public string? Nation { get; set; } = default!;
    public string NewFirstName { get; set; } = default!;
    public string NewLastName { get; set; } = default!;
    public string NewJobTitle { get; set; } = default!;
    public string OldFirstName { get; set; } = default!;
    public string OldLastName { get; set; } = default!;
    public string OldJobTitle { get; set; } = default!;
    public string OrganisationName { get; set; } = default!;
    public string OrganisationNumber { get; set; } = default!;
    public Guid ExternalIdReference { get; set; }
    public string? ContactTelephone { get; set; } = default!;
    public string? ContactEmailAddress { get; set; } = default!;
}
