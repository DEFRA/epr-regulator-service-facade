using EPR.RegulatorService.Facade.API.Handlers;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EPR.RegulatorService.Facade.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class OrganisationRegistrationController( ILogger<OrganisationRegistrationController> logger, ICommonDataService commonDataService) : ControllerBase
    {
        public IOrganisationRegistrationHandlers RegistrationHandler { get; set; } = new OrganisationRegistrationHandlers(commonDataService, logger);

        /// <summary>
        /// To do: Implement the real connection in _commonDataService
        /// JsonOrganisationRegistrationHandler handles the load and filtering of the dummy data
        /// CommonDataOrganisationRegistrationHandler will handle the request to Synapse when the endpoint is declared
        /// </summary>
        /// <param name="request">Filter parameters</param>
        /// <returns>BadRequest or OK</returns>
        [HttpGet]
        [Route("registrations/get-organisations")]
        public async Task<IActionResult> GetOrganisationRegistrations([FromQuery, Required] OrganisationRegistrationFilter request)
        {
            try
            {
                return RegistrationHandler.ValidateIncomingModels(ModelState) ?? await RegistrationHandler.HandleGetOrganisationRegistrations((GetOrganisationRegistrationRequest)request);
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An error was received processing registrations/get-organisations.");
                return HandleError.HandleErrorWithStatusCode(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}
