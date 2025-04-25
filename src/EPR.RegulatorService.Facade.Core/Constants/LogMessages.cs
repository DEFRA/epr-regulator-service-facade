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
    public const string ReprocessingInputsOutputsRegistrationMaterial = "Fetching reprocessing inputs, outputs, and process details for registration material ID: {id}.";
    public const string SamplingPlanRegistrationMaterial = "Fetching sampling plan details for registration material ID: {id}.";
}
