using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.API.Handlers;

public class OrganisationRegistrationHandlers(ICommonDataService commonDataService, ILogger logger)
{
    private readonly JsonSerializerOptions outputSerialisationOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

    public async Task<ActionResult> HandleGetOrganisationRegistrations(GetOrganisationRegistrationRequest filterRequest)
    {
        var registrations = await commonDataService.GetOrganisationRegistrations<JsonOrganisationRegistrationHandler>(filterRequest);

        if (registrations.IsSuccessStatusCode)
        {
            var stringContent = await registrations.Content.ReadAsStringAsync();
            var paginatedResponse = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSummaryResponse>>(stringContent, outputSerialisationOptions);
            return new OkObjectResult(paginatedResponse);
        }

        logger.LogWarning("Didn't fetch Dummy data successfully");
        return HandleError.HandleErrorWithStatusCode(registrations.StatusCode);
    }

    public ActionResult? ManageModelState(ModelStateDictionary modelState)
    {
        if (!modelState.IsValid)
        {
            var validationProblem = new ValidationProblemDetails(modelState)
            {
                Title = "Validation Error",
                Status = 400, // Ensure the status is explicitly set
                Detail = "One or more validation errors occurred.",
            };
            return new BadRequestObjectResult(validationProblem);
        }

        return null;
    }

}
