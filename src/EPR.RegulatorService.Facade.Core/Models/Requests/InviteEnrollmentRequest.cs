using System.Diagnostics.CodeAnalysis;

namespace EPR.RegulatorService.Facade.Core.Models.Requests
{
    [ExcludeFromCodeCoverage]
    public class InviteEnrollmentRequest
    {
        public string Email { get; set; } = null!;
        public int PersonRoleId { get; set; }
        public int ServiceRoleId { get; set; }
        public string RoleKey { get; set; } = null!;
        public Guid OrganisationId { get; set; }
        public Guid? UserId { get; set; }
    }
}
