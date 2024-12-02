﻿using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public interface IOrganisationRegistrationSubmissionService
{
    Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> HandleGetRegistrationSubmissionList(GetOrganisationRegistrationSubmissionsFilter filter);

    Task<RegistrationSubmissionOrganisationDetailsResponse?> HandleGetOrganisationRegistrationSubmissionDetails(Guid submissionId);

    Task<HttpResponseMessage> HandleCreateRegulatorDecisionSubmissionEvent(RegulatorDecisionCreateRequest request, Guid userId);

    Task<HttpResponseMessage> HandleCreateRegistrationFeePaymentSubmissionEvent(RegistrationFeePaymentCreateRequest request, Guid userId);

    string GenerateReferenceNumber( CountryName countryName, RegistrationSubmissionType registrationSubmissionType, string organisationId, string twoDigitYear = null, MaterialType materialType = MaterialType.None);
}