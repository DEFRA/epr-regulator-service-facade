using System.Net.Http.Json;
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
    private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>> HandleGetRegistrationSubmissionList(
        GetOrganisationRegistrationSubmissionsCommonDataFilter filter, Guid userId)
    {
        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse = [];
        DateTime? lastSyncTime = null;

        try
        {
            lastSyncTime = await GetLastSyncTime();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred getting the last Sync time in {MethodName}.", nameof(HandleGetRegistrationSubmissionList));
            throw;
        }

        try
        {
            if (lastSyncTime.HasValue)
            {
                deltaRegistrationDecisionsResponse = await GetDeltaSubmissionEvents(lastSyncTime, userId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred getting the latest submissionsevents {MethodName}.", nameof(HandleGetRegistrationSubmissionList));
        }

        try
        {
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


    private async Task<DateTime?> GetLastSyncTime()
    {
        var lastSyncResponse = await commonDataService.GetSubmissionLastSyncTime();
        if (lastSyncResponse is null || !lastSyncResponse.IsSuccessStatusCode)
        {
            return null;
        }

        var submissionEventsLastSync = lastSyncResponse.Content.ReadFromJsonAsync<SubmissionEventsLastSync>().Result;
        return submissionEventsLastSync.LastSyncTime;
    }


    public async Task<RegistrationSubmissionOrganisationDetailsResponse?> HandleGetOrganisationRegistrationSubmissionDetails(Guid submissionId, Guid userId)
    {
        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse = [];

        var lastSyncTime = await GetLastSyncTime();

        if (lastSyncTime.HasValue)
        {
            deltaRegistrationDecisionsResponse = await GetDeltaSubmissionEvents(lastSyncTime, userId, submissionId);
        }

        var requestedItem = await commonDataService.GetOrganisationRegistrationSubmissionDetails(submissionId);

        if (deltaRegistrationDecisionsResponse.Count > 0)
        {
            MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedItem);
        }

        return requestedItem;
    }

    public async Task<HttpResponseMessage> HandleCreateRegulatorDecisionSubmissionEvent(
        RegulatorDecisionCreateRequest request, Guid userId)
    {
        var regRefNumber =
            request.Status == RegistrationSubmissionStatus.Granted &&
            request.CountryName.HasValue &&
            request.RegistrationSubmissionType.HasValue
                ? GenerateReferenceNumber(
                    request.CountryName.Value,
                    request.RegistrationSubmissionType.Value,
                    request.OrganisationAccountManagementId.ToString(),
                    request.TwoDigitYear)
                : string.Empty;

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
                DecisionDate = request.DecisionDate
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
}