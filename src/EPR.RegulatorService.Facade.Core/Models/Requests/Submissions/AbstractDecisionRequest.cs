using System.ComponentModel.DataAnnotations;
using EPR.RegulatorService.Facade.Core.Attributes;
using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

public abstract class AbstractDecisionRequest
{
    [NotDefault]
    public Guid SubmissionId { get; set; }
    
    [Required]
    public RegulatorDecision Decision { get; set; }
    
    public string? Comments { get; set; }

    [NotDefault]
    public Guid FileId { get; set; }
    
    public Guid OrganisationId { get; set; }
    
    public string OrganisationNumber { get; set; }
    
    public string OrganisationName { get; set; }
}