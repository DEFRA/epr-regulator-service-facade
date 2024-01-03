using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Organisations;

namespace EPR.RegulatorService.Facade.Core.Services.Messaging;

public interface IMessagingService
{
    string? ApprovedPersonAccepted(ApplicationEmailModel model);
    List<string> ApprovedPersonRejected(ApplicationEmailModel model);
    List<string> DelegatedPersonAccepted(ApplicationEmailModel model);
    List<string> DelegatedPersonRejected(ApplicationEmailModel model);
    List<string> SubmissionAccepted(SubmissionEmailModel model);
    List<string> SubmissionRejected(SubmissionEmailModel model, bool resubmissionRequired);
    string? SendRemovedApprovedPersonNotification(AssociatedPersonResults model, int serviceRoleId);
}
