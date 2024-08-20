namespace EPR.RegulatorService.Facade.Core.Services.Application;

using EPR.RegulatorService.Facade.Core.Models.Accounts;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using Models.Applications;

public interface IApplicationService
{
    Task<HttpResponseMessage> PendingApplications(Guid userId, int currentPage, int pageSize, string organisationName, string applicationType);

    Task<HttpResponseMessage> GetOrganisationPendingApplications(Guid userId, Guid organisationId);

    Task<HttpResponseMessage> UpdateEnrolment(ManageRegulatorEnrolmentRequest request);

    Task<HttpResponseMessage> TransferOrganisationNation(OrganisationTransferNationRequest request);
    
    Task<HttpResponseMessage> GetUserOrganisations(Guid userId);
    Task<HttpResponseMessage> AcceptOrRejectUserDetailChangeRequestAsync(UpdateUserDetailRequest request);

    Task<HttpResponseMessage> GetPendingUserDetailChangeRequestsAsync(Guid userId, int currentPage, int pageSize, string organisationName, string applicationType);
    Task<HttpResponseMessage> GetUserDetailChangeRequestAsync(Guid externalId);


}
