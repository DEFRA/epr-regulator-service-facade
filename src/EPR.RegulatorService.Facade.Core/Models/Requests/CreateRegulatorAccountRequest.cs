using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.Core.Models.Requests
{
    public class CreateRegulatorAccountRequest
    {

        [Required]
        public string ServiceId { get; set; }

        [Required]
        [MaxLength(160)]
        public string Name { get; set; } = null!;

        [Required]
        public int NationId { get; set; }
    }
}
