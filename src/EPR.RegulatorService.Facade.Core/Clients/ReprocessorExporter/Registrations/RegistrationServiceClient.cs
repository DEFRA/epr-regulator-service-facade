﻿using EPR.RegulatorService.Facade.Core.Clients;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;

public class RegistrationServiceClient(
HttpClient httpClient,
IOptions<PrnBackendServiceApiConfig> options,
ILogger<RegistrationServiceClient> logger)
: BaseHttpClient(httpClient), IRegistrationServiceClient
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

    public async Task<RegistrationOverviewDto> GetRegistrationByRegistrationId(int id)
    {
        logger.LogInformation(LogMessages.RegistrationMaterialsTasks);
        var url = string.Format($"{_config.Endpoints.RegistrationByRegistrationId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationOverviewDto>(url);
    }

    public async Task<RegistrationMaterialDetailsDto> GetRegistrationMaterialByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.SummaryInfoMaterial);
        var url = string.Format($"{_config.Endpoints.RegistrationMaterialByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialDetailsDto>(url);
    }

    public async Task<RegistrationAccreditationReferenceDto> GetRegistrationAccreditationReference(int id)
    {
        logger.LogInformation(LogMessages.RegistrationAccreditationReference, id);
        var url = string.Format($"{_config.Endpoints.RegistrationAccreditationReference}", _config.ApiVersion, id);
        return await GetAsync<RegistrationAccreditationReferenceDto>(url);
    }

    public async Task<bool> UpdateMaterialOutcomeByRegistrationMaterialId(int id, UpdateMaterialOutcomeWithReferenceDto request)
    {
        logger.LogInformation(LogMessages.OutcomeMaterialRegistration);
        var url = string.Format($"{_config.Endpoints.UpdateMaterialOutcomeByRegistrationMaterialId}", _config.ApiVersion, id);
        return await PostAsync<UpdateMaterialOutcomeWithReferenceDto, bool>(url, request);
    }

    public async Task<RegistrationMaterialWasteLicencesDto> GetWasteLicenceByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.WasteLicencesRegistrationMaterial, id);
        var url = string.Format($"{_config.Endpoints.WasteLicensesByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialWasteLicencesDto>(url);
    }

    public async Task<RegistrationMaterialReprocessingIODto> GetReprocessingIOByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.ReprocessingIORegistrationMaterial, id);
        var url = string.Format($"{_config.Endpoints.ReprocessingIOByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialReprocessingIODto>(url);
    }

    public async Task<RegistrationMaterialSamplingPlanDto> GetSamplingPlanByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.SamplingPlanRegistrationMaterial, id);
        var url = string.Format($"{_config.Endpoints.SamplingPlanByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationMaterialSamplingPlanDto>(url);
    }

    public async Task<RegistrationSiteAddressDto> GetSiteAddressByRegistrationId(int id)
    {
        logger.LogInformation(LogMessages.AttemptingSiteAddressDetails);
        var url = string.Format($"{_config.Endpoints.SiteAddressByRegistrationId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationSiteAddressDto>(url);
    }

    public async Task<MaterialsAuthorisedOnSiteDto> GetAuthorisedMaterialByRegistrationId(int id)
    {
        logger.LogInformation(LogMessages.AttemptingAuthorisedMaterial);
        var url = string.Format($"{_config.Endpoints.AuthorisedMaterialByRegistrationId}", _config.ApiVersion, id);
        return await GetAsync<MaterialsAuthorisedOnSiteDto>(url);
    }

    public async Task<RegistrationFeeContextDto> GetRegistrationFeeRequestByRegistrationMaterialId(int id)
    {
        logger.LogInformation(LogMessages.AttemptingRegistrationFeeDetails);
        var url = string.Format($"{_config.Endpoints.RegistrationFeeByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<RegistrationFeeContextDto>(url);
    }

    public async Task<bool> MarkAsDulyMadeByRegistrationMaterialId(int id, MarkAsDulyMadeWithUserIdDto request)
    {
        logger.LogInformation(LogMessages.AttemptingMarkAsDulyMade);
        var url = string.Format(_config.Endpoints.MarkAsDulyMadeByRegistrationMaterialId, _config.ApiVersion, id);
        return await PostAsync<MarkAsDulyMadeWithUserIdDto, bool>(url, request);
    }
}
