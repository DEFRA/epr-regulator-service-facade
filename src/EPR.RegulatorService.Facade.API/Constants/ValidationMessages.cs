namespace EPR.RegulatorService.Facade.API.Constants;
public static class ValidationMessages
{
    public const string InvalidRegistrationStatus = "Invalid registration material status.";
    public const string RegistrationCommentsMaxLength = "RegistrationMaterial Comment cannot exceed 500 characters.";
    public const string RegistrationCommentsRequired = "Comments are required."; 
    public const string OfflineReferenceRequired = "The Reference field is required.";
    public const string OfflineRegulatorRequired = "The Regulator field is required.";
    public const string AmountRequiredAndGreaterThanZero = "Amount is required and must be greater than zero.";
    public const string InvalidDulyMadeDate = "Duly Made date is mandatory and must be a valid date.";
    public const string InvalidDeterminationDate = "Determination date is mandatory and must be a valid date.";
}
