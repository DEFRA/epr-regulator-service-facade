﻿using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;

public class ReprocessorExporterServiceClient(
HttpClient httpClient,
IOptions<PrnBackendServiceApiConfig> options,
ILogger<ReprocessorExporterServiceClient> logger)
: BaseHttpClient(httpClient), IReprocessorExporterServiceClient
{
    private readonly PrnBackendServiceApiConfig _config = options.Value;

    public async Task<bool> UpdateRegulatorRegistrationTaskStatus(UpdateRegulatorRegistrationTaskDto request)
    {
        logger.LogInformation(LogMessages.UpdateRegistrationTaskStatus, request.Status.ToString());
        var url = string.Format(_config.Endpoints.UpdateRegulatorRegistrationTaskStatusById, _config.ApiVersion);
        return await PostAsync<UpdateRegulatorRegistrationTaskDto, bool>(url, request);
    }

    public async Task<bool> UpdateRegulatorApplicationTaskStatus(UpdateRegulatorApplicationTaskDto request)
    {
        logger.LogInformation(LogMessages.UpdateApplicationTaskStatus, request.Status.ToString());
        var url = string.Format(_config.Endpoints.UpdateRegulatorApplicationTaskStatusById, _config.ApiVersion);
        return await PostAsync<UpdateRegulatorApplicationTaskDto, bool>(url, request);
    }

    public async Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(Guid id)
    {
        logger.LogInformation(LogMessages.RegistrationMaterialsTasks);
        var url = string.Format($"{_config.Endpoints.RegistrationByRegistrationId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationOverviewDto>(url);
    }

    public async Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.SummaryInfoMaterial);
        var url = string.Format($"{_config.Endpoints.RegistrationMaterialByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialDetailsDto>(url);
    }

    public async Task<RegistrationAccreditationReferenceDto> GetRegistrationAccreditationReference(Guid id)
    {
        logger.LogInformation(LogMessages.RegistrationAccreditationReference, id);
        var url = string.Format($"{_config.Endpoints.RegistrationAccreditationReference}", _config.ApiVersion, id);
        return await GetAsync<RegistrationAccreditationReferenceDto>(url);
    }

    public async Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(Guid id, UpdateMaterialOutcomeWithReferenceDto request)
    {
        logger.LogInformation(LogMessages.OutcomeMaterialRegistration);
        var url = string.Format($"{_config.Endpoints.UpdateMaterialOutcomeByRegistrationMaterialId}", _config.ApiVersion, id);
        return await PostAsync<UpdateMaterialOutcomeWithReferenceDto, bool>(url, request);
    }

    public async Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.WasteLicencesRegistrationMaterial, id);
        var url = string.Format($"{_config.Endpoints.WasteLicensesByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialWasteLicencesDto>(url);
    }

    public async Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.ReprocessingIORegistrationMaterial, id);
        var url = string.Format($"{_config.Endpoints.ReprocessingIOByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialReprocessingIODto>(url);
    }

    public async Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.SamplingPlanRegistrationMaterial, id);
        var url = string.Format($"{_config.Endpoints.SamplingPlanByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialSamplingPlanDto>(url);
    }

    public async Task<RegistrationSiteAddressDto> GetSiteAddressByRegistrationId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingSiteAddressDetails);
        var url = string.Format($"{_config.Endpoints.SiteAddressByRegistrationId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationSiteAddressDto>(url);
    }

    public async Task<RegistrationWasteCarrierDto> GetWasteCarrierDetailsByRegistrationId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingWasteCarrierDetails);
        var url = string.Format($"{_config.Endpoints.WasteCarrierDetailsByRegistrationId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationWasteCarrierDto>(url);
    }

    public async Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingAuthorisedMaterial);
        var url = string.Format($"{_config.Endpoints.AuthorisedMaterialByRegistrationId}", _config.ApiVersion, id);
        return await GetAsync<MaterialsAuthorisedOnSiteDto>(url);
    }

    public async Task<RegistrationFeeContextDto> GetRegistrationFeeRequestByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingRegistrationFeeDetails);
        var url = string.Format($"{_config.Endpoints.RegistrationFeeByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationFeeContextDto>(url);
    }

    public async Task<bool> MarkAsDulyMadeByRegistrationMaterialId(Guid id, MarkAsDulyMadeWithUserIdDto request)
    {
        logger.LogInformation(LogMessages.AttemptingMarkAsDulyMade);
        var url = string.Format(_config.Endpoints.MarkAsDulyMadeByRegistrationMaterialId, _config.ApiVersion, id);
        return await PostAsync<MarkAsDulyMadeWithUserIdDto, bool>(url, request);
    }

    public async Task<RegistrationOverviewDto> GetRegistrationByIdWithAccreditationsAsync(Guid id, int? year)
    {
        logger.LogInformation(LogMessages.RegistrationAccreditationTasks);
        var url = string.Format($"{_config.Endpoints.RegistrationByIdWithAccreditations}", _config.ApiVersion, id);
        if (year != null)
        {
            url = $"{url}?year={year}";
        }
        return await GetAsync<RegistrationOverviewDto>(url);
    }

    public async Task<AccreditationSamplingPlanDto> GetSamplingPlanByAccreditationId(Guid id)
    {
        logger.LogInformation(LogMessages.SamplingPlanAccreditation);
        var url = string.Format($"{_config.Endpoints.SamplingPlanByAccreditationId}", _config.ApiVersion, id);
        return await GetAsync<AccreditationSamplingPlanDto>(url);
    }
    
    public async Task<AccreditationFeeContextDto> GetAccreditationPaymentFeeDetailsByAccreditationId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingAccreditationFeeDetails);
        var url = string.Format($"{_config.Endpoints.AccreditationFeeByAccreditationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<AccreditationFeeContextDto>(url);
    }

    public async Task<bool> MarkAsDulyMadeByAccreditationId(Guid id, MarkAsDulyMadeWithUserIdDto request)
    {
        logger.LogInformation(LogMessages.AttemptingMarkAccreditationMaterialAsDulyMade);
        var url = string.Format(_config.Endpoints.MarkAsDulyMadeByAccreditationId, _config.ApiVersion, id);
        return await PostAsync<MarkAsDulyMadeWithUserIdDto, bool>(url, request);
    }

    public async Task<bool> UpdateRegulatorAccreditationTaskStatus(UpdateAccreditationTaskStatusWithUserIdDto request)
    {
        logger.LogInformation(LogMessages.UpdateRegulatorAccreditationTaskStatus, request.Status.ToString());
        var url = string.Format(_config.Endpoints.UpdateRegulatorAccreditationTaskStatusById, _config.ApiVersion);
        return await PostAsync<UpdateAccreditationTaskStatusWithUserIdDto, bool>(url, request);
    }

    public async Task<bool> SaveApplicationTaskQueryNotes(Guid id, QueryNoteRequestDto request)
    {
        logger.LogInformation(LogMessages.AttemptingApplicationTaskQueryNotesSave);
        var url = string.Format(_config.Endpoints.SaveApplicationTaskQueryNotes, _config.ApiVersion, id);
        return await PostAsync<QueryNoteRequestDto, bool>(url, request);
    }

    public async Task<bool> SaveRegistrationTaskQueryNotes(Guid id, QueryNoteRequestDto request)
    {
        logger.LogInformation(LogMessages.AttemptingApplicationTaskQueryNotesSave);
        var url = string.Format(_config.Endpoints.SaveRegistrationTaskQueryNotes, _config.ApiVersion, id);
        return await PostAsync<QueryNoteRequestDto, bool>(url, request);
    }
}