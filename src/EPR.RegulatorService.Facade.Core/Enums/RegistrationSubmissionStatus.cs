using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegistrationSubmissionStatus
{
    None = 0,

    Pending = 1,

    Granted = 10,

    Refused = 20,

    Queried = 30,

    Cancelled = 40,

    Updated = 50,

    Accepted = 60,

    Rejected = 70
}