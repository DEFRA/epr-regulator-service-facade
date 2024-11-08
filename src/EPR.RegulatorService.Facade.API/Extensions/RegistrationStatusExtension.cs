using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.API.Extensions;

internal static class RegistrationStatusExtension
{
    internal static RegulatorDecision GetRegulatorDecision(this RegistrationSubmissionStatus registrationStatus)
    {
        return registrationStatus switch
        {
            RegistrationSubmissionStatus.Pending => RegulatorDecision.None,
            RegistrationSubmissionStatus.Granted => RegulatorDecision.Accepted,
            RegistrationSubmissionStatus.Refused => RegulatorDecision.Rejected,
            RegistrationSubmissionStatus.Queried => RegulatorDecision.Queried,
            RegistrationSubmissionStatus.Cancelled => RegulatorDecision.Cancelled,
            _ => RegulatorDecision.None,
        };
    }
}