using System.ComponentModel;
using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegistrationJourneyType
{
    [Description("RegistrationJourneyType.NotSpecified")]
    Unknown,

    [Description("RegistrationJourneyType.CsoLegacy")]
    CsoLegacy,

    [Description("RegistrationJourneyType.CsoLargeProducer")]
    CsoLargeProducer,

    [Description("RegistrationJourneyType.CsoSmallProducer")]
    CsoSmallProducer,

    [Description("RegistrationJourneyType.DirectLargeProducer")]
    DirectLargeProducer,

    [Description("RegistrationJourneyType.DirectSmallProducer")]
    DirectSmallProducer
}
