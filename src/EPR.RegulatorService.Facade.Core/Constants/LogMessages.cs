using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Constants;

public static class LogMessages
{
    public const string UpdateRegulatorRegistrationTaskStatus = "Attempting to update regulator registration task status";
    public const string UpdateRegulatorApplicationTaskStatus = "Attempting to update regulator application task status";
    public const string RegistrationMaterialsTasks = "Attempting to get registration with materials and tasks";
    public const string SummaryInfoMaterial = "Attempting to get summary info for a material";
    public const string OutcomeMaterialRegistration = "Attempting to update the outcome of a material registration";
    public const string UpdateRegistrationTaskStatus = "Attempting to update regulator registration task status using the backend for Status {Status}";
    public const string UpdateApplicationTaskStatus = "Attempting to update regulator application task status using the backend for Status {Status}";
    public const string WasteLicencesRegistrationMaterial = "Retrieving waste permit and exemption details for registration material with ID {id}.";
    public const string ReprocessingIORegistrationMaterial = "Fetching reprocessing inputs, outputs, and process details for registration material ID: {id}.";
    public const string SamplingPlanRegistrationMaterial = "Fetching sampling plan details for registration material ID: {id}.";
    public const string SiteAddressDetails = "Attempting to get site address details.";
    public const string AuthorisedMaterial = "Attempting to get authorised materials details.";
    public const string RegulatorRegistrationDownloadFile = "Attempting to download a file for registrations.";
    public const string AttemptingSiteAddressDetails = "Attempting to get site address details.";
    public const string AttemptingAuthorisedMaterial = "Attempting to get authorised materials details.";
    public const string AttemptingRegistrationPaymentFee = "Attempting to get payment fee.";
    public const string AttemptingRegistrationFeeDetails = "Attempting to get registration fee details.";
    public const string AttemptingOrganisationName = "Attempting to get organisation name.";
    public const string SaveOfflinePayment = "Save offline payment";
    public const string AttemptingMarkAsDulyMade = "Attempting to mark a registration material as duly made.";
    public const string RegistrationAccreditationReference = "Retrieving registration or Accreditation reference number informations with ID {id}.";
    public const string RegistrationAccreditationTasks = "Attempting to get registration accreditations and tasks";
    public const string AttemptingApplicationTaskQueryNotesSave = "Attempting to save application task query notes";
    public const string AttemptingRegistrationTaskQueryNotesSave = "Attempting to update registration task query notes";
}
