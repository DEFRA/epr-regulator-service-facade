namespace EPR.RegulatorService.Facade.Core.Configs;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class BlobStorageConfig
{
    public const string SectionName = "BlobStorage";

    [Required]
    public string ConnectionString { get; set; }

    [Required]
    public string PomContainerName { get; set; }

    [Required]
    public string RegistrationContainerName { get; set; }

    [Required]
    public string ReprocessorExporterRegistrationContainerName { get; set; }

    [Required]
    public string ReprocessorExporterAccreditationContainerName { get; set; }
}