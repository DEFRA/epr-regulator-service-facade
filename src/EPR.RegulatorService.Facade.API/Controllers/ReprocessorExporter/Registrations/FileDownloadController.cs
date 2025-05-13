using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [FeatureGate(FeatureFlags.ReprocessorExporter)]
    public class FileDownloadController(
        IBlobStorageService blobStorageService,
        IAntivirusService antivirusService,
        ILogger<FileDownloadController> logger) : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly IAntivirusService _antivirusService = antivirusService;

        [HttpPost("regulatorRegistrationDownloadFile")]
        [SwaggerOperation(
            Summary = "Downloads a file from Azure blob storage.",
            Description = "Attempting to download a file from Azure blob storage."
        )]
        public async Task<IActionResult> DownloadFile([FromBody] FileDownloadRequestDto request)
        {
            logger.LogInformation(LogMessages.RegulatorRegistrationDownloadFile, request.FileName);
            var stream = await _blobStorageService.DownloadFileStreamAsync("test-container", request.BlobName);

            // send FileDownloadRequest to Trade antivirus API for checking
            var userId = User.UserId();
            var email = User.Email();
            var truncatedFileName = FileHelpers.GetTruncatedFileName(request.FileName, FileConstants.FileNameTruncationLength);
            
            var antiVirusResponse = await _antivirusService.SendFile("test-container", request.FileId, truncatedFileName, stream, userId, email);
            var antiVirusResult = await antiVirusResponse.Content.ReadAsStringAsync();

            // if clean, return file
            if (antiVirusResult == ContentScan.Clean)
            {
                var file = new FileContentResult(stream.ToArray(), "text/csv")
                {
                    FileDownloadName = request.FileName
                };

                return file;
            }

            return new ObjectResult("The file was found but it was flagged as infected. It will not be downloaded.") { StatusCode = StatusCodes.Status403Forbidden };
        }
    }
}
