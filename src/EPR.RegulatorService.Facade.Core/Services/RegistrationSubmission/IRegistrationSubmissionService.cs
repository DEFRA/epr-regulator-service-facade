

using EPR.RegulatorService.Facade.Core.Enums;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public interface IRegistrationSubmissionService
{
    string GenerateReferenceNumber(CountryName countryName, RegistrationSubmissionType registrationSubmissionType, string organisationId, string twoDigitYear = null, MaterialType materialType = MaterialType.None);
}
