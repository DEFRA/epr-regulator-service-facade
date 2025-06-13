using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData;

[ExcludeFromCodeCoverage]
public class PaycalParametersResponse
{
    public bool IsCso { get; set; }

    public int RelevantYear { get; set; }

    public bool IsLateFee { get; set; }

    public char OrganisationSize { get; set; }

    public string OrganisationType { get; set; } = string.Empty;

    public DateTime FirstSubmittedDate { get; set; }

    public DateTime? JoinerDate { get; set; }

    public DateTime? LeaverDate { get; set; }

    public string? LeaverCode { get; set; }
}