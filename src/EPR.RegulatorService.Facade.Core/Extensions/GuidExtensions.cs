namespace EPR.RegulatorService.Facade.Core.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsValidGuid(this Guid guidValue)
        {
            Guid validUserId;
            return !((!Guid.TryParse(guidValue.ToString(), out validUserId)) || validUserId == Guid.Empty);
        }
    }
}
