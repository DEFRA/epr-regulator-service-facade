using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace EPR.RegulatorService.Facade.API.Controllers
{
    public abstract class FileDownloadBaseController : ControllerBase
    {
        private readonly IAntivirusService _antivirusService;

        protected FileDownloadBaseController(IAntivirusService antivirusService)
        {
            _antivirusService = antivirusService ?? throw new ArgumentNullException(nameof(antivirusService));
        }

        protected async Task<FileContentResult> AntiVirusScanFile(AntiVirusDetails antiVirusDetails, MemoryStream memoryStream)
        {
            var antiVirusResponse = await _antivirusService.SendFile(antiVirusDetails, memoryStream);
            var antiVirusResult = await antiVirusResponse.Content.ReadAsStringAsync();

            FileContentResult fileContentResult = null;

            if (antiVirusResult == ContentScan.Clean)
            {
                fileContentResult = new FileContentResult(memoryStream.ToArray(), "text/csv")
                {
                    FileDownloadName = antiVirusDetails.FileName
                };

                return fileContentResult;
            }
            return fileContentResult;
        }
    }
}
