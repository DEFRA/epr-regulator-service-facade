using EPR.RegulatorService.Facade.Core.Models.PrnBackends;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Clients.PrnBackendServiceClient
{
    public interface IPrnBackendServiceClient
    {
        Task<List<PrnBackendModel>> GetAllPrnByOrganisationId(Guid orgId);//This method needs removing when the test is completed. This method is only used for testing
    }
}
