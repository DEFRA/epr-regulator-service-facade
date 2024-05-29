using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Submissions.Events;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using EPR.RegulatorService.Facade.Core.Services.TradeAntiVirus;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace EPR.RegulatorService.Facade.API.Controllers;

[ApiController]
[Route("api/downloads")]
public class FileDownloadController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ISubmissionService _submissionService;
    private readonly IAntivirusService _antivirusService;

    public FileDownloadController(
        IBlobStorageService blobStorageService,
        ISubmissionService submissionService,
        IAntivirusService antivirusService)
    {
        _blobStorageService = blobStorageService;
        _submissionService = submissionService;
        _antivirusService = antivirusService;
    }

    [HttpPost]
    [Route("file-download")]
    public async Task<IActionResult> DownloadFile([FromBody] FileDownloadRequest request)
    {
        // Get file from blob storage
        var stream = await _blobStorageService.DownloadFileStreamAsync(request.SubmissionType, request.BlobName);

        // send FileDownloadRequest to Trade antivirus API for checking
        var userId = User.UserId();
        var email = User.Email();
        var truncatedFileName = FileHelpers.GetTruncatedFileName(request.FileName, FileConstants.FileNameTruncationLength);
        var antiVirusResponse = await _antivirusService.SendFile(request.SubmissionType, request.FileId, truncatedFileName, stream, userId, email);
        var antiVirusResult = await antiVirusResponse.Content.ReadAsStringAsync();

        // Create a new submissions event for the download attempt
        var fileDownloadCheckEvent = new FileDownloadCheckEvent()
        {
            UserEmail = email,
            ContentScan = antiVirusResult,
            FileId = request.FileId,
            FileName = request.FileName,
            BlobName = request.BlobName,
            SubmissionType = request.SubmissionType
        };

        var submissionEvent = await _submissionService.CreateSubmissionEvent(request.SubmissionId, fileDownloadCheckEvent, User.UserId());
        if (!submissionEvent.IsSuccessStatusCode)
        {
            return new BadRequestObjectResult("There was an error communicating with the submissions API.");
        }

        // if clean, return file
        if (antiVirusResult == ContentScan.Clean)
        {
            var file = new FileContentResult(stream.ToArray(), "text/csv")
            {
                FileDownloadName = request.FileName
            };

            return file;
        }

        return new ObjectResult("The file was found but it was flagged as infected. It will not be downloaded.") {  StatusCode = StatusCodes.Status403Forbidden };
    }
}