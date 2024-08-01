using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Models.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;

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
                    string logData = $"Create regulator organisation name {request.Name} service response is successful";
                    _logger.LogInformation(logData);

                    string headerValue = response.Headers.GetValues("Location").First();

                    string nationName = headerValue.Split('=')[1];

                    var createdOrganisation = await _httpClient.GetAsync($"{_config.Endpoints.GetRegulator}{nationName}");

                    if (createdOrganisation.IsSuccessStatusCode)
                    {
                        string content = await createdOrganisation.Content.ReadAsStringAsync();

                        logData = $"Create regulator organisation name {request.Name} service content response is {content}";
                        LogInformation(logData);

                        var result = JsonSerializer.Deserialize<CreateRegulatorOrganisationResponseModel>(content)!;

                        result.Nation = nationName;

                        logData = $"Create regulator organisation name {request.Name} service nation is {nationName}";
                        LogInformation(logData);

                        return Result<CreateRegulatorOrganisationResponseModel>.SuccessResult(result);
                    }
                    else
                    {
                        logData = $"Get regulator organisation service failed: {createdOrganisation}";
                        LogError(logData, null);
                    }
                }

                return Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex);

                return Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> RegulatorInvites(AddInviteUserRequest request)
        {
            LogInformation("Attempting to fetch pending applications from the backend");

            return await _httpClient.PostAsync(_config.Endpoints.RegulatorInvitation, GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorEnrollment(EnrolInvitedUserRequest request)
        {
            LogInformation("Attempting to fetch pending applications from the backend");

            return await _httpClient.PostAsync(_config.Endpoints.RegulatorEnrollment, GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorInvited(Guid id, string email)
        {
            LogInformation("Attempting to fetch pending invite token from the backend");

            var url = string.Format($"{_config.Endpoints.RegulatorInvitedUser}", id, email);

            return await _httpClient.GetAsync(url);
        }
        
        public async Task<HttpResponseMessage> GetRegulatorUserList(Guid userId, Guid organisationId, bool getApprovedUsersOnly)
        {
            var url = $"{_config.Endpoints.GetRegulatorUsers}?userId={userId}&organisationId={organisationId}&getApprovedUsersOnly={true}";

            string logData = $"Attempting to fetch the users for organisation id {organisationId} from the backend";
            LogInformation(logData);


            return await _httpClient.GetAsync(url);
        }

        private StringContent GetStringContent(object request)
        {
            string jsonRequest = JsonSerializer.Serialize(request);

            return new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        }
        
        public async Task<HttpResponseMessage> GetUsersByOrganisationExternalId(Guid userId, Guid externalId)
        {
            var url = string.Format($"{_config.Endpoints.GetUsersByOrganisationExternalId}", userId, externalId);

            string logData = $"Attempting to fetch the users for organisation external id {externalId} from the backend";
            LogInformation(logData);
        
            return await _httpClient.GetAsync(url);
        }
        
        public async Task<HttpResponseMessage> AddRemoveApprovedUser(AddRemoveApprovedUserRequest request)
        {
            _logger.LogInformation("Attempting to fetch pending applications from the backend");

            return await _httpClient.PostAsync(_config.Endpoints.AddRemoveApprovedUser, GetStringContent(request));
        }

        private void LogInformation(string data)
        {
            if (data != null)
            {
                data = data.Replace('\n', '_').Replace('\r', '_');
                _logger.LogInformation(data);
            }
        }

        private void LogError(string data, Exception ex)
        {
            if (data != null)
            {
                data = data.Replace('\n', '_').Replace('\r', '_');
                _logger.LogError(data, ex);
            }
        }
    }
}
