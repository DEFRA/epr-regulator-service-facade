
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;

public enum RegistrationSubmissionOrganisationType
{
    [Description("RegistrationSubmissionOrganisationType.NotSpecified")]
    none = 0,
    [Description("RegistrationSubmissionOrganisationType.ComplianceScheme")]
    compliance = 1,
    [Description("RegistrationSubmissionOrganisationType.LargeProducer")]
    large = 10,
    [Description("RegistrationSubmissionOrganisationType.SmallProducer")]
    small = 20
}