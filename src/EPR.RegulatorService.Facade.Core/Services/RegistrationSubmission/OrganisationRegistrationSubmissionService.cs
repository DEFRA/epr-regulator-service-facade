using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public class OrganisationRegistrationSubmissionService(
             ICommonDataService commonDataService,
             ISubmissionService submissionService,
             ILogger<OrganisationRegistrationSubmissionService> logger) : IOrganisationRegistrationSubmissionService
{
    private readonly JsonSerializerOptions outputSerialisationOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<IActionResult> HandleGetOrganisationRegistrations(GetOrganisationRegistrationSubmissionsFilter filterRequest)
    {
        var registrations = await commonDataService.GetOrganisationRegistrationSubmissionlist(filterRequest);

        if (registrations.IsSuccessStatusCode)
        {
            var stringContent = await registrations.Content.ReadAsStringAsync();
            var paginatedResponse = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>>(stringContent, outputSerialisationOptions);
            return new OkObjectResult(paginatedResponse);
        }

        logger.LogError("Didn't fetch Dummy data successfully");

        return HandleError.HandleErrorWithStatusCode(registrations.StatusCode);
    }

    public string GenerateReferenceNumber(CountryName countryName, RegistrationSubmissionType registrationSubmissionType, string organisationId, string twoDigitYear = null, MaterialType materialType = MaterialType.None)
    {
        if (string.IsNullOrEmpty(twoDigitYear))
        {
            twoDigitYear = (DateTime.Now.Year % 100).ToString("D2");
        }

        if (string.IsNullOrEmpty(organisationId))
        {
            throw new ArgumentNullException(nameof(organisationId));
        }

        var countryCode = ((char)countryName).ToString();

        var regType = ((char)registrationSubmissionType).ToString();

        string refNumber = $"R{twoDigitYear}{countryCode}{regType}{organisationId}{Generate4DigitNumber()}";

        if (registrationSubmissionType == RegistrationSubmissionType.Reprocessor || registrationSubmissionType == RegistrationSubmissionType.Exporter)
        {
            refNumber = $"{refNumber}{materialType.GetDisplayName<MaterialType>()}";
        }

        return refNumber;
    }
    public ActionResult? ValidateIncomingModels(ModelStateDictionary modelState)
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

    private static string Generate4DigitNumber()
    {
        var min = 1000;
        var max = 10000;
        var randomNumber = RandomNumberGenerator.GetInt32(min, max);

        return randomNumber.ToString();
    }
}