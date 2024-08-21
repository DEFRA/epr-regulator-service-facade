using System;

namespace EPR.RegulatorService.Facade.Core.Extensions
{
    public static class UrlBuilderExtention
    {
        private static readonly string[] allowedSchemes = { "https", "http" };

        public static string FormatURL(string baseAddress, string url)
        {
            if (baseAddress == null) { return string.Empty; }

            UriBuilder uriBuilder = new UriBuilder(baseAddress);
            
            if (!allowedSchemes.Contains(uriBuilder.Scheme))
            {
                throw new ArgumentException();
            }

            if (!string.IsNullOrEmpty(baseAddress) && string.IsNullOrEmpty(url))
            {
                if (baseAddress.EndsWith('/'))
                {
                    return baseAddress;
                }
                return baseAddress + "/";
            }
            else if (String.IsNullOrEmpty(url) || url.StartsWith("http")) 
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
