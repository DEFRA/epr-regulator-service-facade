namespace EPR.RegulatorService.Facade.Core.Models
{
    public class ServiceRolesLookupModel
    {
        public string Key { get; set; } = null!;

        public int ServiceRoleId { get; set; }

        public int PersonRoleId { get; set; }
    }
}
