using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.Core.Attributes;
using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Requests;

public class FileDownloadRequest
{
    [NotDefault]
    public Guid SubmissionId { get; set; }

    [NotDefault]
    public Guid FileId { get; set; }

    [Required]
    public string BlobName { get; set; }

    public string FileName { get; set; }

    [Required]
    public SubmissionType SubmissionType { get; set; }
}
