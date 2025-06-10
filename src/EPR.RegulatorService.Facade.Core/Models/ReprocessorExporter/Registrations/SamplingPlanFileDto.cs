
namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;

public abstract class SamplingPlanFileDto
{
    public required string Filename { get; set; }
    public required string FileId { get; set; }
    public DateTime? DateUploaded { get; set; }
    public string? UpdatedBy { get; set; }
    public required string FileUploadType { get; set; }
    public required string FileUploadStatus { get; set; }
}
