namespace EPR.RegulatorService.Facade.Core.Models.Requests
{
    public class RegulatorInviteEnrollmentRequest : InviteEnrollmentRequest
    {
        public Guid UserId { get; set; }
    }
}
