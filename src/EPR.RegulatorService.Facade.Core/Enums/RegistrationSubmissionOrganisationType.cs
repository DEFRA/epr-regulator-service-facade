
using System.ComponentModel;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;


namespace EPR.RegulatorService.Facade.Core.Enums;

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