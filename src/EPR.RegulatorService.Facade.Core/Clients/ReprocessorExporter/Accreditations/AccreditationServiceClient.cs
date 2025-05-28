using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Accreditations;

public class AccreditationServiceClient(
HttpClient httpClient,
IOptions<PrnBackendServiceApiConfig> options,
ILogger<AccreditationServiceClient> logger)
: BaseHttpClient(httpClient), IAccreditationServiceClient
{
    private readonly PrnBackendServiceApiConfig _config = options.Value;
    
    public async Task<AccreditationFeeContextDto> GetAccreditationFeeRequestByRegistrationMaterialId(Guid id)
    {
        logger.LogInformation(LogMessages.AttemptingAccreditationFeeDetails);
        var url = string.Format($"{_config.Endpoints.AccreditationFeeByRegistrationMaterialId}", _config.ApiVersion, id);
        return await GetAsync<AccreditationFeeContextDto>(url);
    }

    public async Task<bool> MarkAccreditationMaterialStatusAsDulyMade(AccreditationMarkAsDulyMadeWithUserIdDto request)
    {
        logger.LogInformation(LogMessages.AttemptingMarkAccreditationMaterialAsDulyMade);
        var url = string.Format(_config.Endpoints.AccreditationMarkAsDulyMadeByRegistrationMaterialId, _config.ApiVersion);
        return await PostAsync<AccreditationMarkAsDulyMadeWithUserIdDto, bool>(url, request);
    }
    
    public async Task<bool> UpdateAccreditationMaterialTaskStatus(UpdateAccreditationMaterialTaskStatusWithUserIdDto request)
    {
        logger.LogInformation(LogMessages.UpdateAccreditationMaterialTaskStatus, request.TaskStatus.ToString());
        var url = string.Format(_config.Endpoints.UpdateAccreditationMaterialTaskStatus, _config.ApiVersion);
        return await PostAsync<UpdateAccreditationMaterialTaskStatusDto, bool>(url, request);
    }

}
