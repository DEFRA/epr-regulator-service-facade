namespace EPR.RegulatorService.Facade.Core.Models.Applications;

public class UpdateEnrolmentRequest
{
    public Guid EnrolmentId { get; set; }

    public string EnrolmentStatus { get; set; }

    public string Comments { get; set; }
}