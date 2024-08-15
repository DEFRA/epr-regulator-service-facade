using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

[ExcludeFromCodeCoverage]
public class EprPackagingRegulatorEmailConfig
{
    public const string SectionName = "EprPackagingRegulatorEmailConfig";
    public string England { get; set; } = string.Empty;
    public string Wales { get; set; } = string.Empty;
    public string Scotland { get; set; } = string.Empty;
    public string NorthernIreland { get; set; } = string.Empty;

}