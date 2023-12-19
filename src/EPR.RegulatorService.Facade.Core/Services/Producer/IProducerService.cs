namespace EPR.RegulatorService.Facade.Core.Services.Producer;

/// <summary>
/// This service provides data for managing producers
/// </summary>
public interface IProducerService
{
    Task<HttpResponseMessage> GetOrganisationsBySearchTerm(Guid userId, int currentPage, int pageSize, string searchTerm);

    Task<HttpResponseMessage> GetOrganisationDetails(Guid userId, Guid externalId);
    
   
    Task<HttpResponseMessage> RemoveApprovedUser(Guid userId, Guid connExternalId, Guid organisationId);
}
