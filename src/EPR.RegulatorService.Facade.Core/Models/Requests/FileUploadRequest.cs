using EPR.RegulatorService.Facade.Core.Enums;
using System;

namespace EPR.RegulatorService.Facade.Core.Models.Requests
{
    public class FileUploadRequest
    {
        public SubmissionType SubmissionType { get; set; }
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public byte[] FileContent { get; set; }
    }
}
