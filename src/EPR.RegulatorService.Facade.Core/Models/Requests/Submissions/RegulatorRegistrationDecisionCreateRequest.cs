using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.Core.Attributes;
using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions
{
    public class RegulatorRegistrationDecisionCreateRequest
    {
        [NotDefault]
        public Guid SubmissionId { get; set; }
    
        [NotDefault]
        public Guid FileId { get; set; }
    
        [Required]
        public RegulatorDecision Decision { get; set; }
    
        public string? Comments { get; set; }
    }
}