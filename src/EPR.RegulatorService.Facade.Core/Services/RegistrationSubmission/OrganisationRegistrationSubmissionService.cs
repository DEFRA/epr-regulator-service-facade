﻿using System.Net.Http.Json;
using System.Text.Json;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Submissions;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.Extensions.Logging;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;

public partial class OrganisationRegistrationSubmissionService(
    ICommonDataService commonDataService,
    ISubmissionService submissionService,
    ILogger<OrganisationRegistrationSubmissionService> logger) : IOrganisationRegistrationSubmissionService
{

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> HandleGetRegistrationSubmissionList(
        GetOrganisationRegistrationSubmissionsFilter filter, Guid userId)
    {
        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse = [];
        DateTime? lastSyncTime = null;

        try
        {
            lastSyncTime = await GetLastSyncTime();

            if (lastSyncTime.HasValue)
            {
                deltaRegistrationDecisionsResponse = await GetDeltaSubmissionEvents(lastSyncTime, userId);
            }

            if (deltaRegistrationDecisionsResponse.Count > 0 && !string.IsNullOrWhiteSpace(filter.Statuses))
            {
                ApplyAppRefNumbersForRequiredStatuses(deltaRegistrationDecisionsResponse, filter.Statuses, filter);
            }

            PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> requestedList =
                    await commonDataService.GetOrganisationRegistrationSubmissionList(filter);

            if (deltaRegistrationDecisionsResponse.Count > 0)
            {
                MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);
            }

            requestedList.items = [.. requestedList.items.OrderBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Cancelled)
                .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Refused)
                .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Granted && !x.IsResubmission )
                .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Queried)
                .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Granted && x.ResubmissionStatus == RegistrationSubmissionStatus.Rejected)
                .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Granted && x.ResubmissionStatus == RegistrationSubmissionStatus.Accepted)
                .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Granted && x.ResubmissionStatus == RegistrationSubmissionStatus.Pending)
                .ThenBy(x => x.SubmissionStatus == RegistrationSubmissionStatus.Pending)
                .ThenByDescending(x => x.SubmissionDate)];

            return requestedList;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred getting the latest submissions {MethodName}.", nameof(HandleGetRegistrationSubmissionList));
            return new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
            {
                currentPage = 1,
                pageSize = filter.PageSize ?? 20,
                totalItems = 0,
                items = []
            };
        }
    }

    public async Task<RegistrationSubmissionOrganisationDetailsFacadeResponse?> HandleGetOrganisationRegistrationSubmissionDetails(Guid submissionId, Guid userId)
    {
        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse = [];

        var lastSyncTime = await GetLastSyncTime();

        if (lastSyncTime.HasValue)
        {
            deltaRegistrationDecisionsResponse = await GetDeltaSubmissionEvents(lastSyncTime, userId, submissionId);
        }

        var requestedItem = await commonDataService.GetOrganisationRegistrationSubmissionDetails(submissionId);

        if (deltaRegistrationDecisionsResponse.Count > 0 && requestedItem is not null)
        {
            MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedItem);
        }

        return requestedItem;
    }

    public async Task<HttpResponseMessage> HandleCreateRegulatorDecisionSubmissionEvent(
        RegulatorDecisionCreateRequest request, Guid userId)
    {
        var regRefNumber = string.Empty;
        if (request.Status == RegistrationSubmissionStatus.Granted)
        {
            regRefNumber = request.ExistingRegRefNumber;
            if (!request.IsResubmission && request.CountryName.HasValue && request.RegistrationSubmissionType.HasValue)
            {
                regRefNumber = GenerateReferenceNumber(
                    request.CountryName.Value,
                    request.RegistrationSubmissionType.Value,
                    request.ApplicationReferenceNumber,
                    request.OrganisationAccountManagementId.ToString(),
                    request.TwoDigitYear);
            }
        }

        return await submissionService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationSubmissionDecisionEvent
            {
                ApplicationReferenceNumber = request.ApplicationReferenceNumber,
                OrganisationId = request.OrganisationId,
                SubmissionId = request.SubmissionId,
                Decision = request.Status.GetRegulatorDecision(),
                Comments = request.Comments,
                RegistrationReferenceNumber = regRefNumber,
                DecisionDate = request.DecisionDate,
                FileId = request.FileId
            },
            userId
        );
    }


    public async Task<HttpResponseMessage> HandleCreateRegistrationFeePaymentSubmissionEvent(RegistrationFeePaymentCreateRequest request, Guid userId)
    {
        return await submissionService.CreateSubmissionEvent(
            request.SubmissionId,
            new RegistrationFeePaymentEvent
            {
                SubmissionId = request.SubmissionId,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = request.PaymentStatus,
                PaidAmount = $"£{request.PaidAmount}"
            },
            userId
        );
    }

    public async Task<HttpResponseMessage> HandleCreatePackagingDataResubmissionFeePaymentEvent(PackagingDataResubmissionFeePaymentCreateRequest request, Guid userId)
    {
        return await submissionService.CreateSubmissionEvent(
            request.SubmissionId,
            new PackagingDataResubmissionFeePaymentEvent
            {
                SubmissionId = request.SubmissionId,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = request.PaymentStatus,
                PaidAmount = $"£{request.PaidAmount}"
            },
            userId
        );
    }

    private async Task<DateTime?> GetLastSyncTime()
    {
        var lastSyncResponse = await commonDataService.GetSubmissionLastSyncTime();
        if (lastSyncResponse is null || !lastSyncResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var submissionEventsLastSync = await lastSyncResponse.Content.ReadFromJsonAsync<SubmissionEventsLastSync>();
        return submissionEventsLastSync.LastSyncTime;
    }
}