﻿using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Models.Results;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.Core.Services.Regulator
{
    public class RegulatorOrganisationService : IRegulatorOrganisationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RegulatorOrganisationService> _logger;
        private readonly AccountsServiceApiConfig _config;
        
        public RegulatorOrganisationService(
            HttpClient httpClient,
            ILogger<RegulatorOrganisationService> logger,
            IOptions<AccountsServiceApiConfig> config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _config = config.Value;
        }

        public async Task<CheckRegulatorOrganisationExistResponseModel?> GetRegulatorOrganisationByNation(string nation)
        {
            var response = await _httpClient.GetAsync($"{_config.Endpoints.GetRegulator}{nation}");

            if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
            {
                string result = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<CheckRegulatorOrganisationExistResponseModel>(result)!;
            }
            return null;
        }

        public async Task<Result<CreateRegulatorOrganisationResponseModel>> CreateRegulatorOrganisation(CreateRegulatorAccountRequest request)
        {
            try
            {
                string jsonRequest = JsonSerializer.Serialize(request);

                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_config.Endpoints.CreateRegulator, stringContent);

                if (response.IsSuccessStatusCode)
                {
                    string logData = string.Format("Create regulator organisation name {0} service response is successful", request.Name);
                    logData = logData.Replace('\n', '_');
                    _logger.LogInformation("{Message}", logData);

                    string headerValue = response.Headers.GetValues("Location").First();

                    string nationName = headerValue.Split('=')[1];

                    var createdOrganisation = await _httpClient.GetAsync($"{_config.Endpoints.GetRegulator}{nationName}");

                    if (createdOrganisation.IsSuccessStatusCode)
                    {
                        string content = await createdOrganisation.Content.ReadAsStringAsync();

                        logData = string.Format("Create regulator organisation name {0} service response is {1}", request.Name, content).Replace('\n', '_');
                        _logger.LogInformation("{Message}", logData);
                        
                        var result = JsonSerializer.Deserialize<CreateRegulatorOrganisationResponseModel>(content)!;

                        result.Nation = nationName;

                        logData = string.Format("Create regulator organisation name {0} service nation is {1}", request.Name, nationName).Replace('\n', '_');
                        _logger.LogInformation("{Message}", logData);

                        return Result<CreateRegulatorOrganisationResponseModel>.SuccessResult(result);
                    }
                    else
                    {
                        logData = string.Format("Get regulator organisation service failed: {0}", createdOrganisation).Replace('\n', '_');
                        _logger.LogInformation("{Message}", logData);
                    }
                }

                return Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                return Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> RegulatorInvites(AddInviteUserRequest request)
        {
            _logger.LogInformation("Attempting to fetch pending applications from the backend");

            return await _httpClient.PostAsync(_config.Endpoints.RegulatorInvitation, GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorEnrollment(EnrolInvitedUserRequest request)
        {
            _logger.LogInformation("Attempting to fetch pending applications from the backend");

            return await _httpClient.PostAsync(_config.Endpoints.RegulatorEnrollment, GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorInvited(Guid id, string email)
        {
            _logger.LogInformation("Attempting to fetch pending applications from the backend");

            var url = string.Format($"{_config.Endpoints.RegulatorInvitedUser}", id, email);

            var uriBuilder = new UriBuilder(_httpClient.BaseAddress)
            {
                Path = url
            };

            return await _httpClient.GetAsync(uriBuilder.Uri.LocalPath);
        }
        
        public async Task<HttpResponseMessage> GetRegulatorUserList(Guid userId, Guid organisationId, bool getApprovedUsersOnly)
        {
            var url = $"{_config.Endpoints.GetRegulatorUsers}?userId={userId}&organisationId={organisationId}&getApprovedUsersOnly={true}";

            string logData = string.Format($"Attempting to fetch the users for organisation id {0} from the backend", organisationId);
            _logger.LogInformation("{Message}", logData);

            return await _httpClient.GetAsync(url);
        }

        private static StringContent GetStringContent(object request)
        {
            string jsonRequest = JsonSerializer.Serialize(request);

            return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        }
        
        public async Task<HttpResponseMessage> GetUsersByOrganisationExternalId(Guid userId, Guid externalId)
        {
            var url = string.Format($"{_config.Endpoints.GetUsersByOrganisationExternalId}", userId, externalId);

            string logData = string.Format($"Attempting to fetch the users for organisation external id {0} from the backend", externalId);
            _logger.LogInformation("{Message}", logData);

            var uriBuilder = new UriBuilder(_httpClient.BaseAddress)
            {
                Path = url
            };

            return await _httpClient.GetAsync(uriBuilder.Path);
        }
        
        public async Task<HttpResponseMessage> AddRemoveApprovedUser(AddRemoveApprovedUserRequest request)
        {
            _logger.LogInformation("Attempting to fetch pending applications from the backend");

            return await _httpClient.PostAsync(_config.Endpoints.AddRemoveApprovedUser, GetStringContent(request));
        }
    }
}
