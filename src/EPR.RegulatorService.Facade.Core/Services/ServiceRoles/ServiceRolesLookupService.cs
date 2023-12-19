using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Services.ServiceRoles
{
    public class ServiceRolesLookupService : IServiceRolesLookupService
    {
        private readonly ServiceRolesConfig _config;

        public ServiceRolesLookupService(IOptions<ServiceRolesConfig> config) => _config = config.Value;

        public List<ServiceRolesLookupModel> GetServiceRoles()
        {
            return _config.Roles;
        }
    }
}
