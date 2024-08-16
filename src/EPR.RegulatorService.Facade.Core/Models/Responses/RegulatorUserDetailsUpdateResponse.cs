
using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Models.Accounts;

namespace EPR.RegulatorService.Facade.Core.Models.Responses;

[ExcludeFromCodeCoverage]
public class RegulatorUserDetailsUpdateResponse
{
    public bool HasUserDetailsChangeAccepted { get; set; } = false;
    public bool HasUserDetailsChangeRejected { get; set; } = false;

    public ChangeHistoryModel? ChangeHistory { get; set; }
}