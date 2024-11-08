using System.ComponentModel;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum RegistrationStatusX
{
    [Description("Not specified")]
    None = 0,

    [Description("Pending")]
    Pending = 1,

    [Description("Granted")]
    Granted = 10,

    [Description("Refused")]
    Refused = 20,

    [Description("Queried")]
    Queried = 30,

    [Description("Cancelled")]
    Cancelled = 40
}