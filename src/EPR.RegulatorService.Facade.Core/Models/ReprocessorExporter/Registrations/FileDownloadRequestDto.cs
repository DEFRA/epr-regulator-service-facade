using EPR.RegulatorService.Facade.Core.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations
{
    public class FileDownloadRequestDto
    {
        [Required]
        public Guid FileId { get; set; }

        [Required]
        public string FileName { get; set; }
    }
}
