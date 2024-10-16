using System.ComponentModel;

namespace EPR.RegulatorService.Facade.Core.Enums;

public enum OrganisationType
{
    [Description("Not specified")]
    none = 0,
    [Description("Compliance scheme")]
    compliance = 1,
    [Description("Large producer")]
    large = 10,
    [Description("Small producer")]
    small = 20
}
