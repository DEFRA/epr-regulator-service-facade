using EPR.RegulatorService.Facade.Core.Models.PrnBackends;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations
{
    //This Service needs removing when the test is completed. This endpoint is only used for testing
    public interface IRegistrationsService
    {
        Task<List<PrnBackendModel>> GetAllPrnByOrganisationId(Guid orgId);
    }
}
