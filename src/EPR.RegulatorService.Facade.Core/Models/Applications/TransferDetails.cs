using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Applications;

[ExcludeFromCodeCoverage]
public class TransferDetails
{
    public int OldNationId { get; set; }
    public DateTimeOffset TransferredDate { get; set; }
}
