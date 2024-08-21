namespace EPR.RegulatorService.Facade.Core.Extensions
{
    public static class UrlBuilderExtention
    {
        public static string FormatURL(string baseAddress, string url)
        {
            UriBuilder uriBuilder = new UriBuilder(baseAddress);

            if (url.StartsWith("http")) 
            {
                return url;
            }
            else if (uriBuilder.Path.ToString() == "/")
            {
                uriBuilder.Path = url;
            }
            else
            {
                uriBuilder.Path += "/" + url;
            }
            return uriBuilder.Uri.LocalPath;
        }
    }
}
