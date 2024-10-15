using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Services;
using EPR.RegulatorService.Facade.Core.Services.CommonData;

namespace EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;


public interface IOrganisationRegistrationDataSource
{
    Task<HttpResponseMessage> GetOrganisationRegistrations(GetOrganisationRegistrationRequest request);
}
