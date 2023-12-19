namespace EPR.RegulatorService.Facade.Core.Models.Requests
{
    public class AddInviteUserRequest
    {
        public InvitedUser InvitedUser { get; set; } = null!;

        public InvitingUser InvitingUser { get; set; } = null!;
    }

    public class InvitedUser
    {
        public string Email { get; set; } = null!;

        public int PersonRoleId { get; set; }

        public int ServiceRoleId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid? UserId { get; set; } = Guid.Empty!;
    }

    public class InvitingUser
    {
        public string Email { get; set; } = null!;

        public Guid UserId { get; set; }
    }
}
