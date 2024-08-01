namespace EPR.RegulatorService.Facade.Core.Helpers
{
    public static class UrlHelpers
    {
        private static readonly string[] _allowedSchemes = { "https", "http" };
        private static readonly string[] _allowedDomains = { "http://localhost" };

        public static string CheckRequestURL(string location)
        {
            if (_allowedSchemes.Contains(location) ||
                _allowedDomains.Contains(location) ||
                string.IsNullOrEmpty(location))
            {
                return location;
            }
            return location;
        }
    }
}