using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public interface IOrganisationRegistrationSubmissionService
{
    Task<IActionResult> HandleGetOrganisationRegistrations(GetOrganisationRegistrationSubmissionsFilter filterRequest);

    ActionResult? ValidateIncomingModels(ModelStateDictionary modelState);

    string GenerateReferenceNumber(
        CountryName countryName,
        RegistrationSubmissionType
        registrationSubmissionType,
        string organisationId,
        string twoDigitYear = null,
        MaterialType materialType = MaterialType.None);
}