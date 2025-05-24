using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations
{
    public class AccreditedMaterialDto
    {
        public int MaterialId { get; set; }   // RegistrationMaterial.Id
        public string MaterialName { get; set; }
        public AccreditationDetailsDto Accreditation { get; set; }
    }
}
