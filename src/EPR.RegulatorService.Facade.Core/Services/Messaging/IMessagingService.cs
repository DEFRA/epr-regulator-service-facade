﻿using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Models.Organisations;

namespace EPR.RegulatorService.Facade.Core.Services.Messaging;

public interface IMessagingService
{
    string? ApprovedPersonAccepted(ApplicationEmailModel model);

    List<string> ApprovedPersonRejected(ApplicationEmailModel model);

    List<string> DelegatedPersonAccepted(ApplicationEmailModel model);

    List<string> DelegatedPersonRejected(ApplicationEmailModel model);

    List<string> SubmissionAccepted(SubmissionEmailModel model, EventType type);

    List<string> SubmissionRejected(SubmissionEmailModel model, bool? resubmissionRequired);

    string SendEmailToInvitedNewApprovedPerson(AddRemoveNewApprovedPersonEmailModel model);

    string? SendRemovedApprovedPersonNotification(AssociatedPersonResults model, string notificationType);

    void OrganisationRegistrationSubmissionDecision(OrganisationRegistrationSubmissionEmailModel model);

    void OrganisationRegistrationSubmissionQueried(OrganisationRegistrationSubmissionEmailModel model);

    void OrganisationRegistrationResubmissionDecision(OrganisationRegistrationSubmissionEmailModel model);
}
