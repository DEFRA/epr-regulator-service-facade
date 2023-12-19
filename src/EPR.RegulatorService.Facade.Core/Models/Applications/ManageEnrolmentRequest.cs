namespace EPR.RegulatorService.Facade.Core.Models.Applications;

public class ManageRegulatorEnrolmentRequest
{
    public Guid UserId { get; set; }
    public Guid EnrolmentId { get; set; }
    public string EnrolmentStatus { get; set; }
    public string RegulatorComment { get; set; }
}