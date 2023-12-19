using EPR.RegulatorService.Facade.Core.Models;

namespace EPR.RegulatorService.Facade.Core.Configs
{
    public class ServiceRolesConfig
    {
        public const string SectionName = "RolesConfig";

        public List<ServiceRolesLookupModel> Roles { get; set; }
    }
}
