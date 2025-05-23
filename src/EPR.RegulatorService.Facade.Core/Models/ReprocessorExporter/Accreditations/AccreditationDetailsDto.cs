using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations
{
    public class AccreditationDetailsDto
    {
        public int AccreditationId { get; set; }
        public string Status { get; set; }                              // e.g., Pending, Granted
        public DateTime? DeterminationDate { get; set; }
        public List<AccreditationTaskDto> Tasks { get; set; }
    }
}
