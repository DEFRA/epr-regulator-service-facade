using EPR.RegulatorService.Facade.Core.Models;

namespace EPR.RegulatorService.Facade.Core.Services.ServiceRoles
{
    public interface IServiceRolesLookupService
    {
        List<ServiceRolesLookupModel> GetServiceRoles();
    }
}
