namespace EPR.RegulatorService.Facade.Core.Helpers
{
    public static class UrlHelpers
    {
        private static readonly string[] _allowedSchemes = { "https", "http" };
        private static readonly string[] _allowedDomains = { "localhost" };

        public static string CheckRequestURL(string location)
        {
            if (!_allowedSchemes.Contains(location) || string.IsNullOrEmpty(location)) return location;

            Uri uri = new Uri(location);

            if (!_allowedDomains.Contains(uri.Host) && !_allowedSchemes.Contains(uri.Scheme))
            {
                return string.Empty;
            }

            return uri.ToString();
        }
    }
}