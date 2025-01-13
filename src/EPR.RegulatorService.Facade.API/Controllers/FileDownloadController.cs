using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Helpers;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Submissions.Events;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using Microsoft.AspNetCore.Mvc;

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

        return new ObjectResult("The file was found but it was flagged as infected. It will not be downloaded.") {  StatusCode = StatusCodes.Status500InternalServerError };
    }
}