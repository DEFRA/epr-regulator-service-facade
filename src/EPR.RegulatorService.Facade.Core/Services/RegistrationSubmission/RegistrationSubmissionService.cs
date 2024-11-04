

using EPR.RegulatorService.Facade.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public class RegistrationSubmissionService : IRegistrationSubmissionService
{
    public  string GenerateReferenceNumber(CountryName countryName, RegistrationSubmissionType registrationSubmissionType, string organisationId, string twoDigitYear = null, MaterialType materialType = MaterialType.None)
    {
        if (string.IsNullOrEmpty(twoDigitYear))
        {
            twoDigitYear = (DateTime.Now.Year % 100).ToString("D2");
        }

        if (string.IsNullOrEmpty(organisationId))
        {
            throw new ArgumentNullException("organisationId");
        }

        var countryCode = ((char)countryName).ToString();
        var regType = ((char)registrationSubmissionType).ToString();

        string refNumber = $"R{twoDigitYear}{countryCode}{regType}{organisationId}{GenerateRandomNumber()}";

        if (registrationSubmissionType == RegistrationSubmissionType.Reprocessor || registrationSubmissionType == RegistrationSubmissionType.Exporter)
        {
            var stringEnumeratedValue = materialType.GetType().GetMember(materialType.ToString())[0].GetCustomAttribute<DisplayAttribute>()?.GetName();
            refNumber = $"{refNumber}{stringEnumeratedValue}";
        }

        return refNumber;
    }

    private string GenerateRandomNumber()
    { 
        var min = 1000;
        var max = 10000;

        var randomNumber = RandomNumberGenerator.GetInt32(min, max);
        return randomNumber.ToString();
    }
}
