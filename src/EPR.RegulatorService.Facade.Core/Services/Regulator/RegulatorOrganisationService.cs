using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Extensions;
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
        private readonly string[] allowedSchemes = { "https", "http" };

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

            var url = UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), $"{_config.Endpoints.GetRegulator}{nation}");

            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
            {
                string result = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<CheckRegulatorOrganisationExistResponseModel>(result)!;
            }
            return null;
        }

        public async Task<Result<CreateRegulatorOrganisationResponseModel>> CreateRegulatorOrganisation(CreateRegulatorAccountRequest request)
        {
            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            try
            {
                string jsonRequest = JsonSerializer.Serialize(request);

                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), _config.Endpoints.CreateRegulator), stringContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Create regulator organisation name {Name} service response is successful", request.Name);

                    string headerValue = response.Headers.GetValues("Location").First();

                    string nationName = headerValue.Split('=')[1];

                    var createdOrganisation = await _httpClient.GetAsync($"{_config.Endpoints.GetRegulator}{nationName}");

                    if (createdOrganisation.IsSuccessStatusCode)
                    {
                        string content = await createdOrganisation.Content.ReadAsStringAsync();
                        _logger.LogInformation("Create regulator organisation name {Name} service response is {Content}", request.Name, content);

                        var result = JsonSerializer.Deserialize<CreateRegulatorOrganisationResponseModel>(content)!;

                        result.Nation = nationName;

                        _logger.LogInformation("Create regulator organisation name {Name} service nation is {NationName}", request.Name, nationName);
                        return Result<CreateRegulatorOrganisationResponseModel>.SuccessResult(result);
                    }
                    else
                    {
                        _logger.LogError("Get regulator organisation service failed: {CreatedOrganisation}", createdOrganisation);
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
            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            _logger.LogInformation("Attempting to fetch pending applications from the backend");
            
            return await _httpClient.PostAsync(UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), _config.Endpoints.RegulatorInvitation), GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorEnrollment(EnrolInvitedUserRequest request)
        {
            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            _logger.LogInformation("Attempting to fetch pending applications from the backend");

            return await _httpClient.PostAsync(_config.Endpoints.RegulatorEnrollment, GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorInvited(Guid id, string email)
        {
            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            _logger.LogInformation("Attempting to fetch pending applications from the backend");

            var url = string.Format($"{_config.Endpoints.RegulatorInvitedUser}", id, email);

            return await _httpClient.GetAsync(UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), url));
        }
        
        public async Task<HttpResponseMessage> GetRegulatorUserList(Guid userId, Guid organisationId, bool getApprovedUsersOnly)
        {
            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            var url = $"{_config.Endpoints.GetRegulatorUsers}?userId={userId}&organisationId={organisationId}&getApprovedUsersOnly={true}";

            _logger.LogInformation("Attempting to fetch the users for organisation id {OrganisationId} from the backend", organisationId);

            return await _httpClient.GetAsync(UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), url));
        }

        private static StringContent GetStringContent(object request)
        {
            string jsonRequest = JsonSerializer.Serialize(request);

            return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        }
        
        public async Task<HttpResponseMessage> GetUsersByOrganisationExternalId(Guid userId, Guid externalId)
        {
            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            var url = string.Format($"{_config.Endpoints.GetUsersByOrganisationExternalId}", userId, externalId);

            _logger.LogInformation("Attempting to fetch the users for organisation external id {ExternalId} from the backend", externalId);

            return await _httpClient.GetAsync(UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), url));
        }
        
        public async Task<HttpResponseMessage> AddRemoveApprovedUser(AddRemoveApprovedUserRequest request)
        {
            Uri uri = new Uri(_httpClient.BaseAddress.ToString());
            if (!allowedSchemes.Contains(uri.Scheme))
            {
                return null;
            }

            _logger.LogInformation("Attempting to fetch pending applications from the backend");
            
            return await _httpClient.PostAsync(UrlBuilderExtention.FormatURL(_httpClient.BaseAddress.ToString(), _config.Endpoints.AddRemoveApprovedUser), GetStringContent(request));
        }
    }
}
