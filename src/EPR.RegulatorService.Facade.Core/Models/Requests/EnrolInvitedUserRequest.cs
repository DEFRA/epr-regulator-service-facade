namespace EPR.RegulatorService.Facade.Core.Models.Requests
{
    public class EnrolInvitedUserRequest
    {
        public string InviteToken { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string UserId { get; set; } = null!;
    }
}
