using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Models.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
            string url = $"{_config.Endpoints.GetRegulator}{nation}";

            #pragma warning disable S5144
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
            try
            {
                string jsonRequest = JsonSerializer.Serialize(request);

                var stringContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_config.Endpoints.CreateRegulator, stringContent);

                if (response.IsSuccessStatusCode)
                {
                    string logData = String.Format("Create regulator organisation name {0} service response is successful", request.Name);
                    _logger.LogInformation(logData);

                    string headerValue = response.Headers.GetValues("Location").First();

                    string nationName = headerValue.Split('=')[1];

                    var createdOrganisation = await _httpClient.GetAsync($"{_config.Endpoints.GetRegulator}{nationName}");

                    if (createdOrganisation.IsSuccessStatusCode)
                    {
                        string content = await createdOrganisation.Content.ReadAsStringAsync();

                        _logger.LogInformation("Create regulator organisation name {0} service response is {1}", request.Name, content);
                        
                        var result = JsonSerializer.Deserialize<CreateRegulatorOrganisationResponseModel>(content)!;

                        result.Nation = nationName;

                        _logger.LogInformation("Create regulator organisation name {0} service nation is {1}", request.Name, nationName);

                        return Result<CreateRegulatorOrganisationResponseModel>.SuccessResult(result);
                    }
                    else
                    {
                        _logger.LogInformation("Get regulator organisation service failed: {0}", createdOrganisation);
                    }
                }

                return Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);

                return Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> RegulatorInvites(AddInviteUserRequest request)
        {
            string logData = "Attempting to fetch pending applications from the backend";
            _logger.LogInformation(logData);

            return await _httpClient.PostAsync(_config.Endpoints.RegulatorInvitation, GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorEnrollment(EnrolInvitedUserRequest request)
        {
            string logData = "Attempting to fetch pending applications from the backend";
            _logger.LogInformation(logData);

            return await _httpClient.PostAsync(_config.Endpoints.RegulatorEnrollment, GetStringContent(request));
        }

        public async Task<HttpResponseMessage> RegulatorInvited(Guid id, string email)
        {
            string logData = "Attempting to fetch pending applications from the backend";
            _logger.LogInformation(logData);

            var url = string.Format($"{_config.Endpoints.RegulatorInvitedUser}", id, email);

            return await _httpClient.GetAsync(url);
        }
        
        public async Task<HttpResponseMessage> GetRegulatorUserList(Guid userId, Guid organisationId, bool getApprovedUsersOnly)
        {
            var url = $"{_config.Endpoints.GetRegulatorUsers}?userId={userId}&organisationId={organisationId}&getApprovedUsersOnly={true}";

            string logData = $"Attempting to fetch the users for organisation id {organisationId} from the backend";
            _logger.LogInformation(logData);

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
            _logger.LogInformation(logData);

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
