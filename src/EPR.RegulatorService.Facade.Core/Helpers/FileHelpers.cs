namespace EPR.RegulatorService.Facade.Core.Helpers
{
    public static class FileHelpers
    {
        public static string GetTruncatedFileName(string fileName, int truncationLength)
        {
            if (fileName.Length < truncationLength)
            {
                return fileName;
            }

            var extension = Path.GetExtension(fileName);
            var truncatedName = Path.GetFileNameWithoutExtension(fileName).Substring(0, truncationLength - extension.Length);
            return $"{truncatedName}{extension}";
        }
    }
}