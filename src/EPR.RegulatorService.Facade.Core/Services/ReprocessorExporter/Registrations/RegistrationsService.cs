using EPR.RegulatorService.Facade.Core.Clients.PrnBackendServiceClient;
using EPR.RegulatorService.Facade.Core.Models.PrnBackends;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations
{
    //This Service needs removing when the test is completed. This service is only used for testing
    [ExcludeFromCodeCoverage]
    public class RegistrationsService(IPrnBackendServiceClient prnServiceClient, ILogger<RegistrationsService> logger) : IRegistrationsService
    {
        public async Task<List<PrnBackendModel>> GetAllPrnByOrganisationId(Guid orgId)
        {
            logger.LogInformation("RegistrationsService - GetAllPrnByOrganisationId - orgId {OrganisationId}", orgId);
            return await prnServiceClient.GetAllPrnByOrganisationId(orgId);
        }
    }
}
