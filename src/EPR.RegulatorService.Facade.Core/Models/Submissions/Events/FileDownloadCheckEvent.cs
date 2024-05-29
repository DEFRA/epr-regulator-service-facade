using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Submissions.Events;

public class FileDownloadCheckEvent : AbstractEvent
{
    public override EventType Type => EventType.FileDownloadCheck;

    public string UserEmail { get; set; }

    public string ContentScan { get; set; }

    public Guid FileId { get; set; }

    public string FileName { get; set; }

    public string BlobName { get; set; }

    public SubmissionType SubmissionType { get; set; }
}
