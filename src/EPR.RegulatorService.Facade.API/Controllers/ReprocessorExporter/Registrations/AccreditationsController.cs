using Asp.Versioning;
using EPR.RegulatorService.Facade.API.Constants;
using EPR.RegulatorService.Facade.API.Extensions;
using EPR.RegulatorService.Facade.API.Helpers;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.TradeAntiVirus;
using EPR.RegulatorService.Facade.Core.Services.BlobStorage;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.TradeAntiVirus;
using EPR.RegulatorService.Facade.API.Helpers;
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
public class AccreditationsController : FileDownloadBaseController
{

    private readonly IBlobStorageService _blobStorageService;
    private readonly IAntivirusService _antivirusService;
    private readonly BlobStorageConfig _blobStorageConfig;
    private readonly IReprocessorExporterService _reprocessorExporterService;
    private readonly IValidator<MarkAsDulyMadeRequestDto> _markAsDulyMadeRequestDtoValidator;
    private readonly IValidator<UpdateAccreditationTaskStatusDto> _updateAccreditationMaterialTaskValidator;
    private readonly IValidator<OfflinePaymentRequestDto> _offlinePaymentRequestDtoValidator;
    private readonly ILogger<AccreditationsController> _logger;

    public AccreditationsController(
        IReprocessorExporterService reprocessorExporterService,
    IValidator<MarkAsDulyMadeRequestDto> markAsDulyMadeRequestDtoValidator,
    IValidator<UpdateAccreditationTaskStatusDto> updateAccreditationMaterialTaskValidator,
    IValidator<OfflinePaymentRequestDto> offlinePaymentRequestDtoValidator,
    IBlobStorageService blobStorageService,
    IAntivirusService antivirusService,
    IOptions<BlobStorageConfig> blobStorageConfig,
    ILogger<AccreditationsController> logger) : base(antivirusService)
    {
        _reprocessorExporterService = reprocessorExporterService;
        _markAsDulyMadeRequestDtoValidator = markAsDulyMadeRequestDtoValidator;
        _updateAccreditationMaterialTaskValidator = updateAccreditationMaterialTaskValidator;
        _offlinePaymentRequestDtoValidator = offlinePaymentRequestDtoValidator;
        _blobStorageService =  blobStorageService;
        _antivirusService = antivirusService;
        _blobStorageConfig = blobStorageConfig.Value;
        _logger = logger;
    }

    [HttpGet("registrations/{id:Guid}/accreditations")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "get accreditation data for the given registration.",
            Description = "Returns all accreditation data for a given site registration, including material-level and site-level tasks.  "
        )]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetRegistrationByIdWithAccreditationsAsync(Guid id, [FromQuery] int? year)
    {
        _logger.LogInformation(LogMessages.RegistrationAccreditationTasks);
        var accreditations = await _reprocessorExporterService.GetRegistrationByIdWithAccreditationsAsync(id, year);
        return Ok(accreditations);
    }

    [HttpGet("accreditations/{id:Guid}/samplingPlan")]
    [ProducesResponseType(typeof(AccreditationSamplingPlanDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "get sampling data for a given accreditation",
            Description = "Returns all sampling data of an accreditation"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "If the request is successful.", typeof(SamplingPlanFileDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetSamplingPlansAsync(Guid id)
    {
        _logger.LogInformation(LogMessages.SamplingPlanAccreditation);
        var samplingPlans = await _reprocessorExporterService.GetSamplingPlanByAccreditationId(id);

        return Ok(samplingPlans);
    }

    [HttpGet("accreditations/{id:guid}/paymentFees")]
    [ProducesResponseType(typeof(AccreditationPaymentFeeDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Get accreditation fee details by registered material id.",
        Description = "Attempting to get accreditation fee details."
    )]
    
    [SwaggerResponse(StatusCodes.Status200OK, "Returns accreditation fee details.", typeof(AccreditationPaymentFeeDetailsDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetAccreditationPaymentFeeDetailsByAccreditationId(Guid id)
    {
        _logger.LogInformation(LogMessages.AttemptingAccreditationFeeDetails);
        var result = await _reprocessorExporterService.GetAccreditationPaymentFeeDetailsByAccreditationId(id);
        return Ok(result);
    }
    
    [HttpPost("accreditations/{id}/markAsDulyMade")]
    [SwaggerOperation(
        Summary = "Mark a accreditation material as duly made”",
        Description = "Attempting to mark a accreditation material as duly made. "
    )]
    [ApiConventionMethod(typeof(CommonApiConvention), nameof(CommonApiConvention.Post))]
    public async Task<IActionResult> MarkAsDulyMadeByAccreditationId(
        [FromRoute] Guid id,
        [FromBody] MarkAsDulyMadeRequestDto request)
    {
        await _markAsDulyMadeRequestDtoValidator.ValidateAndThrowAsync(request);
        _logger.LogInformation(LogMessages.AttemptingMarkAccreditationMaterialAsDulyMade);
        await _reprocessorExporterService.MarkAsDulyMadeByAccreditationId(id, User.UserId(), request);
        return NoContent();
    }

    [HttpPost("regulatorAccreditationTaskStatus")]
    [SwaggerOperation(
        Summary = "Updates a accreditation task status.",
        Description = "Attempting to update regulator accreditation task status."
    )]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
    [SwaggerResponse(StatusCodes.Status204NoContent, $"Returns No Content", typeof(NoContentResult))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid or a validation error occurs.", typeof(ProblemDetails))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> UpdateRegulatorAccreditationTaskStatus([FromBody] UpdateAccreditationTaskStatusDto request)
    {
        await _updateAccreditationMaterialTaskValidator.ValidateAndThrowAsync(request);
        _logger.LogInformation(LogMessages.UpdateRegulatorAccreditationTaskStatus, request.Status);
        await _reprocessorExporterService.UpdateRegulatorAccreditationTaskStatus(User.UserId(), request);
        return NoContent();
    }


    [HttpPost("accreditations/offlinePayment")]
    [SwaggerOperation(
        Summary = "Saves a new offline payment",
        Description = "Save a new offline payment with mandatory payment request data.  "
    )]
    [ApiConventionMethod(typeof(CommonApiConvention), nameof(CommonApiConvention.Post))]
    public async Task<IActionResult> SaveAccreditationOfflinePayment([FromBody] OfflinePaymentRequestDto request)
    {
        await _offlinePaymentRequestDtoValidator.ValidateAndThrowAsync(request);
        _logger.LogInformation(LogMessages.SaveAccreditationOfflinePayment);
        await _reprocessorExporterService.SaveAccreditationOfflinePayment(User.UserId(), request);
        return NoContent();
    }

    [HttpGet("accreditations/{id:Guid}/businessPlan")]
    [ProducesResponseType(typeof(AccreditationBusinessPlanDto), 200)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
            Summary = "get business plan for a given accreditation",
            Description = "Returns business plan data for an accreditation"
        )]
    [SwaggerResponse(StatusCodes.Status200OK, "If the request is successful.", typeof(AccreditationBusinessPlanDto))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "If an unexpected error occurs.", typeof(ContentResult))]
    public async Task<IActionResult> GetBusinessPlanAsync(Guid id)
    {
        logger.LogInformation(LogMessages.BusinessPlanAccreditation);
        var businessPlan = await reprocessorExporterService.GetBusinessPlanByAccreditationId(id);

        return Ok(businessPlan);
    }

    [HttpPost("accreditations/file-download")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "If the file being downloaded is infected with a virus.", typeof(ObjectResult))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "If an unexpected error occurs.", typeof(ContentResult))]
    [SwaggerOperation(
            Summary = "Downloads a file from Azure blob storage.",
            Description = "Attempting to download a file from Azure blob storage."
        )]
    public async Task<IActionResult> DownloadFile([FromBody] FileDownloadRequestDto request)
    {
        _logger.LogInformation(LogMessages.RegulatorAccreditationDownloadFile);

        var stream = await _blobStorageService.DownloadFileStreamAsync(_blobStorageConfig.ReprocessorExporterAccreditationContainerName,
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