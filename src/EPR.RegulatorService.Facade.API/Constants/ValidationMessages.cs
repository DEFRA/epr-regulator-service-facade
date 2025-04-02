namespace EPR.RegulatorService.Facade.API.Constants;
public static class ValidationMessages
{
    public const string StatusRequired = "Status is required.";
    public const string CommentsRequiredWhenStatusIsQueried = "The Comments field is required when Status is Queried.";
    public const string CommentsMaxLengthError = "Comments must not exceed 200 characters.";
}