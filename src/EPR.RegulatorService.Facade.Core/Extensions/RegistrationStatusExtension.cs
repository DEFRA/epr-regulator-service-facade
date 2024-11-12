using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Extensions;

internal static class RegistrationStatusExtension
{
    public static RegulatorDecision GetRegulatorDecision(this RegistrationSubmissionStatus registrationStatus)
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