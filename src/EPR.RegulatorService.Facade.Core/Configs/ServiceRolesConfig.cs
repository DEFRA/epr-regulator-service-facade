using System.Diagnostics.CodeAnalysis;
using EPR.RegulatorService.Facade.Core.Models;

namespace EPR.RegulatorService.Facade.Core.Configs
{
    [ExcludeFromCodeCoverage]
    public class ServiceRolesConfig
    {
        public const string SectionName = "RolesConfig";

        public List<ServiceRolesLookupModel> Roles { get; set; }
    }
}
