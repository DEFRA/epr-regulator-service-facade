using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Services.Regulator;

namespace EPR.RegulatorService.Facade.API.Shared
{
    public class RegulatorUsers<T> : IRegulatorUsers where T : class
    {
        private readonly ILogger<T> _logger;
        private readonly IRegulatorOrganisationService _regulatorOrganisationService;
    
        public RegulatorUsers(IRegulatorOrganisationService regulatorOrganisationService, ILogger<T> logger)
        {
            _regulatorOrganisationService = regulatorOrganisationService;
            _logger = logger;
        }
        
        public async Task<List<OrganisationUser>> GetRegulatorUsers(Guid userId, Guid organisationId)
        {
            var response = await _regulatorOrganisationService.GetRegulatorUserList(userId, organisationId, true);
            if (response.IsSuccessStatusCode)
            {
                var userListResponse = response.Content.ReadFromJsonAsync<List<OrganisationUser>>().Result;
    
                _logger.LogInformation("Fetched the users for organisation {OrganisationId}", organisationId);
    
                return userListResponse;
            }
            
            _logger.LogError("Failed to fetch the users for organisation {OrganisationId}", organisationId);
            
            return null;
        }
    }
}