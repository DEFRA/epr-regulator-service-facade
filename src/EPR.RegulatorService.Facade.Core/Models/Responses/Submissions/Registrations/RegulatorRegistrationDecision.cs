namespace EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.Registrations;

public class RegulatorRegistrationDecision
{
    public Guid FileId { get; set; }
    
    public string? Comments { get; set; }

    public string Decision { get; set; }
}