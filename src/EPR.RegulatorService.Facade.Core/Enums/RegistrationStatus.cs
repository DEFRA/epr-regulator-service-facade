using System.ComponentModel;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegistrationStatus
{
    [Description("Not specified")]
    none = 0,
    [Description("Pending")]
    pending=1,
    [Description("Granted")]
    granted=10,
    [Description("Refused")]
    refused=20,
    [Description("Queried")]
    queried=30,
    [Description("Cancelled")]
    cancelled=40
}
