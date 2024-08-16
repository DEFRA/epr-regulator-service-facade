using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests;
[ExcludeFromCodeCoverage]
public class ManageUserDetailsChangeRequest
{
    public Guid UserId { get; set; }

    public Guid ChangeHistoryExternalId { get; set; }

    public bool HasRegulatorAccepted { get; set; } = false;

    public string? RegulatorComment { get; set; }

}