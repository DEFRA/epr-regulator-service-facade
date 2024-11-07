using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.API.Extensions;

internal static class RegistrationStatusExtension
{
    internal static RegulatorDecision GetRegulatorDecision(this RegistrationStatus registrationStatus)
    {
        return registrationStatus switch
        {
            RegistrationStatus.Pending => RegulatorDecision.None,
            RegistrationStatus.Granted => RegulatorDecision.Accepted,
            RegistrationStatus.Refused => RegulatorDecision.Rejected,
            RegistrationStatus.Queried => RegulatorDecision.Queried,
            RegistrationStatus.Cancelled => RegulatorDecision.Cancelled,
            _ => RegulatorDecision.None,
        };
    }
}