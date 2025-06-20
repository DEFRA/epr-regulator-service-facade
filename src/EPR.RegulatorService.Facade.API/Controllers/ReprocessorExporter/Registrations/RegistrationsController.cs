using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace EPR.RegulatorService.Facade.API.Controllers.ReprocessorExporter.Registrations;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[FeatureGate(FeatureFlags.ReprocessorExporter)]
public class RegistrationsController : FileDownloadBaseController
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IAntivirusService _antivirusService;
    private readonly BlobStorageConfig _blobStorageConfig;
    private readonly IReprocessorExporterService _reprocessorExporterService;
    private readonly IValidator<UpdateRegulatorRegistrationTaskDto> _updateRegulatorRegistrationTaskValidator;
    private readonly IValidator<UpdateRegulatorApplicationTaskDto> _updateRegulatorApplicationTaskValidator;
    private readonly IValidator<QueryNoteRequestDto> _queryNoteRequestDtoValidator;
    private readonly ILogger<RegistrationsController> _logger;

    public RegistrationsController(
        IReprocessorExporterService reprocessorExporterService,
        IValidator<UpdateRegulatorRegistrationTaskDto> updateRegulatorRegistrationTaskValidator,
        IValidator<UpdateRegulatorApplicationTaskDto> updateRegulatorApplicationTaskValidator,
        IValidator<QueryNoteRequestDto> queryNoteRequestDtoValidator,
        IBlobStorageService blobStorageService,
        IAntivirusService antivirusService,
        IOptions<BlobStorageConfig> blobStorageConfig,
        ILogger<RegistrationsController> logger) : base(antivirusService)
    {
        _reprocessorExporterService = reprocessorExporterService;
        _updateRegulatorRegistrationTaskValidator = updateRegulatorRegistrationTaskValidator;
        _updateRegulatorApplicationTaskValidator = updateRegulatorApplicationTaskValidator;
        _queryNoteRequestDtoValidator = queryNoteRequestDtoValidator;
        _blobStorageService = blobStorageService;
        _antivirusService = antivirusService;
        _blobStorageConfig = blobStorageConfig.Value;
        _logger = logger;
    }

    [HttpPost("regulatorRegistrationTaskStatus")]
    [SwaggerOperation(
            Summary = "Updates a registration-level task (no associated material).",
            Description = "Attempting to update regulator registration task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> UpdateRegulatorRegistrationTaskStatus([FromBody] UpdateRegulatorRegistrationTaskDto request)
    {
        await _updateRegulatorRegistrationTaskValidator.ValidateAndThrowAsync(request);

        _logger.LogInformation(LogMessages.UpdateRegulatorRegistrationTaskStatus);

        _ = await _reprocessorExporterService.UpdateRegulatorRegistrationTaskStatus(request);

        return NoContent();
    }

    [HttpPost("regulatorApplicationTaskStatus")]
    [SwaggerOperation(
            Summary = "Updates a material-specific task status.",
            Description = "Attempting to update regulator application task status."
        )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> UpdateRegulatorApplicationTaskStatus([FromBody] UpdateRegulatorApplicationTaskDto request)
    {
        await _updateRegulatorApplicationTaskValidator.ValidateAndThrowAsync(request);

        _logger.LogInformation(LogMessages.UpdateRegulatorApplicationTaskStatus);

        _ = await _reprocessorExporterService.UpdateRegulatorApplicationTaskStatus(request);

        return NoContent();
    }

    [HttpGet("registrations/{id}")]
    [ProducesResponseType(typeof(RegistrationOverviewDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
            Summary = "get registration with materials and tasks",
            Description = "attempting to get registration with materials and tasks.  "
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns registration with materials and tasks.", typeof(RegistrationOverviewDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetRegistrationByRegistrationId(Guid id)
    {
        _logger.LogInformation(LogMessages.RegistrationMaterialsTasks);
        var result = await _reprocessorExporterService.GetRegistrationByRegistrationId(id);
        return Ok(result);
    }

    [HttpGet("registrations/{id}/siteAddress")]
    [ProducesResponseType(typeof(SiteAddressDetailsDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "get site address details",
        Description = "attempting to get site address details.  "
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns site address details.", typeof(SiteAddressDetailsDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetSiteAddressByRegistrationId(Guid id)
    {
        _logger.LogInformation(LogMessages.AttemptingSiteAddressDetails);
        var result = await _reprocessorExporterService.GetSiteAddressByRegistrationId(id);
        return Ok(result);
    }

    [HttpGet("registrations/{id}/wasteCarrier")]
    [ProducesResponseType(typeof(RegistrationWasteCarrierDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "get waste carrier details",
        Description = "attempting to get waste carrier details.  "
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns waste carrier details.", typeof(RegistrationWasteCarrierDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetWasteCarrierDetailsByRegistrationId(Guid id)
    {
        _logger.LogInformation(LogMessages.AttemptingWasteCarrierDetails);
        var result = await _reprocessorExporterService.GetWasteCarrierDetailsByRegistrationId(id);
        return Ok(result);
    }

    [HttpGet("registrations/{id}/authorisedMaterials")]
    [ProducesResponseType(typeof(MaterialsAuthorisedOnSiteDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
    Summary = "get materials authorised details",
    Description = "attempting to get authorised materials details.  "
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Returns materials authorised details.", typeof(MaterialsAuthorisedOnSiteDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetAuthorisedMaterialByRegistrationId(Guid id)
    {
        _logger.LogInformation(LogMessages.AttemptingAuthorisedMaterial);
        var result = await _reprocessorExporterService.GetAuthorisedMaterialByRegistrationId(id);
        return Ok(result);
    }

    [HttpPost("regulatorApplicationTaskStatus/{id}/queryNote")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContentResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ContentResult))]
    [SwaggerOperation(
           Summary = "Save query notes to application task",
           Description = "Attempting to save query notes to application task. "
       )]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> SaveApplicationTaskQueryNotes(
       [FromRoute] Guid id,
       [FromBody] QueryNoteRequestDto request)
    {
        await _queryNoteRequestDtoValidator.ValidateAndThrowAsync(request);
        _logger.LogInformation(LogMessages.AttemptingApplicationTaskQueryNotesSave);
        await _reprocessorExporterService.SaveApplicationTaskQueryNotes(id, User.UserId(), request);
        return NoContent();
    }

    [HttpPost("regulatorRegistrationTaskStatus/{id}/queryNote")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(NoContentResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ContentResult))]
    [SwaggerOperation(
          Summary = "Save query notes to registration task",
          Description = "Attempting to save query notes to registration task. "
      )]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> SaveRegistrationTaskQueryNotes(
      [FromRoute] Guid id,
      [FromBody] QueryNoteRequestDto request)
    {
        await _queryNoteRequestDtoValidator.ValidateAndThrowAsync(request);
        _logger.LogInformation(LogMessages.AttemptingApplicationTaskQueryNotesSave);
        await _reprocessorExporterService.SaveRegistrationTaskQueryNotes(id, User.UserId(), request);
        return NoContent();
    }

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
        _logger.LogInformation(LogMessages.RegulatorRegistrationDownloadFile);

        var stream = await _blobStorageService.DownloadFileStreamAsync(_blobStorageConfig.ReprocessorExporterRegistrationContainerName,
                                                                        request.FileId.ToString());

        var antiVirusDetails = new AntiVirusDetails
        {
            FileId = request.FileId,
            FileName = request.FileName,
            SubmissionType = SubmissionType.Registration,
            UserId = User.UserId(),
            UserEmail = User.Email()
        };                 

        var file = await AntiVirusScanFile(antiVirusDetails, stream);

        return file != null
           ? file
           : new ObjectResult("The file was found but it was flagged as infected. It will not be downloaded.") { StatusCode = StatusCodes.Status403Forbidden };
    }
}