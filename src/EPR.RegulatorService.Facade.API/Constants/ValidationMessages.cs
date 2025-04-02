namespace EPR.RegulatorService.Facade.API.Constants;
public static class ValidationMessages
{
    public const string StatusRequired = "Status is required.";
    public const string CommentsRequiredIfStatusIsQueried = "Comments field is required if 'Status' is queried.";
    public const string CommentsMaxLength = "Comments must not exceed 200 characters.";
}