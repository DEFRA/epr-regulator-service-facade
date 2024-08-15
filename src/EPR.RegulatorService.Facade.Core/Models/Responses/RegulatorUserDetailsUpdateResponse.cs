
using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Models.Accounts;

namespace EPR.RegulatorService.Facade.Core.Models.Responses;

[ExcludeFromCodeCoverage]
public class RegulatorUserDetailsUpdateResponse
{
    public bool HasUserDetailsAcceptedOrRejected { get; set; } = false;

    public ChangeHistoryModel? ChangeHistory { get; set; }
}