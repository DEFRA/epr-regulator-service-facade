namespace EPR.RegulatorService.Facade.Core.Services.Application;

using Models.Applications;

public interface IApplicationService
{
    Task<HttpResponseMessage> PendingApplications(Guid userId, int currentPage, int pageSize, string organisationName, string applicationType);

    Task<HttpResponseMessage> GetOrganisationPendingApplications(Guid userId, Guid organisationId);

    Task<HttpResponseMessage> UpdateEnrolment(ManageRegulatorEnrolmentRequest request);

    Task<HttpResponseMessage> TransferOrganisationNation(OrganisationTransferNationRequest request);
    
    Task<HttpResponseMessage> GetUserOrganisations(Guid userId);
}
