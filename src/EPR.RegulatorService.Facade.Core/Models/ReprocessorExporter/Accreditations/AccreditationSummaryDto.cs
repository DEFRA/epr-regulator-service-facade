using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations
{
    public class AccreditationSummaryDto
    {
        public int RegistrationId { get; set; }                         // Parent Registration.Id
        public string OrganisationName { get; set; }
        public List<AccreditedMaterialDto> Materials { get; set; }
        public List<AccreditationTaskDto> SiteLevelTasks { get; set; }
    }
}
