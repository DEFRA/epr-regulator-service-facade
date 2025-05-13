using EPR.RegulatorService.Facade.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class FileDownloadRequestDto
    {
        [NotDefault]
        public Guid FileId { get; set; }

        [Required]
        public string BlobName { get; set; }

        public string FileName { get; set; }
    }
}
