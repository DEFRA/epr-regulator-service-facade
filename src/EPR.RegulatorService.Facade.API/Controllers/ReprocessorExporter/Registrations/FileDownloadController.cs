using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Helpers;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        IOptions<BlobStorageConfig> blobStorageConfig,
        IOptions<AntivirusApiConfig> antivirusApiConfig,
        ILogger<FileDownloadController> logger) : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService = blobStorageService;
        private readonly IAntivirusService _antivirusService = antivirusService;
        private readonly BlobStorageConfig _blobStorageConfig = blobStorageConfig.Value;
        private readonly AntivirusApiConfig _antivirusApiConfig = antivirusApiConfig.Value;

        [HttpPost("registrations/file-download")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "If the file being downloaded is infected with a virus.", typeof(ObjectResult))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
        [SwaggerOperation(
            Summary = "Downloads a file from Azure blob storage.",
            Description = "Attempting to download a file from Azure blob storage."
        )]
        public async Task<IActionResult> DownloadFile([FromBody] FileDownloadRequestDto request)
        {
            logger.LogInformation(LogMessages.RegulatorRegistrationDownloadFile);

            var stream = await _blobStorageService.DownloadFileStreamAsync(_blobStorageConfig.ReprocessorExporterRegistrationContainerName,
                                                                            request.FileName.ToString());

            // send FileDownloadRequest to Trade antivirus API for checking
            var userId = User.UserId();
            var email = User.Email();
            var truncatedFileName = FileHelpers.GetTruncatedFileName(request.FileName, FileConstants.FileNameTruncationLength);
            var suffix = _antivirusApiConfig.CollectionSuffix;

            var antiVirusContainer = AntiVirus.GetContainerName(SubmissionType.Registration.GetDisplayName<SubmissionType>(), suffix);

            var fileDetails = new FileDetails
            {
                Key = request.FileId,
                Extension = Path.GetExtension(request.FileName),
                FileName = Path.GetFileNameWithoutExtension(request.FileName),
                Collection = antiVirusContainer,
                UserId = userId,
                UserEmail = email,
                PersistFile = _antivirusApiConfig.PersistFile
            };

            var antiVirusResponse = await _antivirusService.SendFile(fileDetails, truncatedFileName, stream);
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
