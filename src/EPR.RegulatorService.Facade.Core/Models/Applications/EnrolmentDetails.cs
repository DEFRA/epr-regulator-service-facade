using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Applications;

[ExcludeFromCodeCoverage]
public class EnrolmentDetails
{
    public string EnrolmentStatus { get; set; }

    public Guid ExternalId { get; set; }

    public string ServiceRole { get; set; }
}