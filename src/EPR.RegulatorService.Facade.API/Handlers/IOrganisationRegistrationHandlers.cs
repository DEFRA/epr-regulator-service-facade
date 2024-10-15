using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.API.Handlers
{
    public interface IOrganisationRegistrationHandlers
    {
        Task<ActionResult> HandleGetOrganisationRegistrations(GetOrganisationRegistrationRequest filterRequest);
        ActionResult? ManageModelState(ModelStateDictionary modelState);
    }
}