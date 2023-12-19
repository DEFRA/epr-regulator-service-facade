using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Models.Results;

namespace EPR.RegulatorService.Facade.Core.Services.Regulator
{
    public interface IRegulatorOrganisationService
    {
        Task<Result<CreateRegulatorOrganisationResponseModel>> CreateRegulatorOrganisation(CreateRegulatorAccountRequest request);
        Task<CheckRegulatorOrganisationExistResponseModel?> GetRegulatorOrganisationByNation(string nation);
        Task<HttpResponseMessage> RegulatorInvites(AddInviteUserRequest request);
        Task<HttpResponseMessage> RegulatorEnrollment(EnrolInvitedUserRequest request);
        Task<HttpResponseMessage> RegulatorInvited(Guid id, string email);
        Task<HttpResponseMessage> GetRegulatorUserList(Guid userId, Guid organisationId, bool getApprovedUsersOnly);
        Task<HttpResponseMessage> GetUsersByOrganisationExternalId(Guid userId, Guid externalId);
    }
}
