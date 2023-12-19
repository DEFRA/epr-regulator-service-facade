using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

namespace EPR.RegulatorService.Facade.Core.Services.Messaging;

public interface IMessagingService
{
    string? ApprovedPersonAccepted(ApplicationEmailModel model);
    List<string> ApprovedPersonRejected(ApplicationEmailModel model);
    List<string> DelegatedPersonAccepted(ApplicationEmailModel model);
    List<string> DelegatedPersonRejected(ApplicationEmailModel model);
    List<string> SubmissionAccepted(SubmissionEmailModel model);
    List<string> SubmissionRejected(SubmissionEmailModel model, bool resubmissionRequired);
}
