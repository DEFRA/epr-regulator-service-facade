using EPR.RegulatorService.Facade.API.Constants;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace EPR.RegulatorService.Facade.API.Helpers
{
    public static class AntiVirus
    {
        public static FileContentResult CheckAntiVirusScan(string? antiVirusResult, MemoryStream? memoryStream, string fileName)
        {
            FileContentResult fileContentResult = null;
           
            if (antiVirusResult == ContentScan.Clean)
            {
                fileContentResult = new FileContentResult(memoryStream.ToArray(), "text/csv")
                {
                    FileDownloadName = fileName
                };

                return fileContentResult;
            }
            return fileContentResult;
        }
    }
}
