using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
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

public class OrganisationRegistrationSubmissionService(
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

    private static void MergeCosmosUpdates(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> requestedList)
    {
        foreach (var item in requestedList.items)
        {
            var cosmosItems = deltaRegistrationDecisionsResponse.Where(x => !string.IsNullOrWhiteSpace(x.AppReferenceNumber) && x.AppReferenceNumber.Equals(item?.ApplicationReferenceNumber, StringComparison.OrdinalIgnoreCase));
            if (cosmosItems.Any())
            {
                foreach (var cosmosItem in cosmosItems.Where(x => x.Type.Equals("RegulatorRegistrationDecision", StringComparison.OrdinalIgnoreCase)))
                {
                    item.RegulatorCommentDate = cosmosItem.Created;
                    item.RegistrationReferenceNumber = cosmosItem.RegistrationReferenceNumber ?? item.RegistrationReferenceNumber;
                    item.StatusPendingDate = cosmosItem.DecisionDate;
                    item.SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
                }
                foreach (var cosmosItem in cosmosItems.Where(x => x.Type.Equals("RegistrationApplicationSubmitted", StringComparison.OrdinalIgnoreCase)))
                {
                    item.RegulatorCommentDate = cosmosItem.Created;
                    item.StatusPendingDate = cosmosItem.DecisionDate;
                    item.ProducerCommentDate = cosmosItem.Created;
                    if (cosmosItem.Created > item.RegulatorCommentDate)
                    {
                        item.SubmissionStatus = RegistrationSubmissionStatus.Updated;
                    }
                }
            }
        }
    }

    private static void MergeCosmosUpdates(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, RegistrationSubmissionOrganisationDetailsResponse item)
    {
        var cosmosItems = deltaRegistrationDecisionsResponse.Where(x => !string.IsNullOrWhiteSpace(x.AppReferenceNumber) && x.AppReferenceNumber.Equals(item.ApplicationReferenceNumber, StringComparison.OrdinalIgnoreCase));

        if (cosmosItems.Any())
        {
            foreach (var cosmosItem in cosmosItems)
            {
                if (cosmosItem.Type.Equals("RegulatorRegistrationDecision", StringComparison.OrdinalIgnoreCase))
                {
                    item.RegulatorComments = cosmosItem.Comments;
                    item.RegulatorDecisionDate = cosmosItem.Created;
                    item.StatusPendingDate = cosmosItem.DecisionDate;
                    item.SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
                    item.SubmissionDetails.Status = item.SubmissionStatus;
                    item.RegistrationReferenceNumber = cosmosItem.RegistrationReferenceNumber;
                }
                else if (cosmosItem.Type.Equals("RegistrationApplicationSubmitted", StringComparison.OrdinalIgnoreCase))
                {
                    item.ProducerComments = cosmosItem.Comments;
                    item.ProducerCommentDate = cosmosItem.Created;
                    if (cosmosItem.Created > item.RegulatorDecisionDate)
                    {
                        item.SubmissionStatus = RegistrationSubmissionStatus.Updated;
                        item.SubmissionDetails.Status = item.SubmissionStatus;
                    }
                }
            }
        }
    }

    private static void ApplyAppRefNumbersForRequiredStatuses(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, string statuses, GetOrganisationRegistrationSubmissionsCommonDataFilter filter)
    {
        filter.ApplicationReferenceNumbers = string.Join(" ",
            statuses
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(status => status.Trim())
                .Where(status => deltaRegistrationDecisionsResponse.Exists(x => x.Decision == status))
                .SelectMany(status => deltaRegistrationDecisionsResponse
                    .Where(x => x.Decision == status)
                    .Select(x => x.AppReferenceNumber))
        );
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

    public class AbstractCosmosSubmissionEvent
    {
        public string AppReferenceNumber { get; set; }
        public DateTime Created { get; set; }
        public string Comments { get; set; }
        public string Decision { get; set; }
        public DateTime? DecisionDate { get; set; }
        public string RegistrationReferenceNumber { get; set; }
        public Guid SubmissionId { get; set; }
        public string Type { get; set; }
    }

    private async Task<List<AbstractCosmosSubmissionEvent>> GetDeltaSubmissionEvents(DateTime? lastSyncTime, Guid userId, Guid? SubmissionId = null)
    {
        List<AbstractCosmosSubmissionEvent> results = [];

        var deltaRegistrationDecisionsResponse = await submissionService.GetDeltaOrganisationRegistrationEvents(lastSyncTime.Value, userId, SubmissionId);

        if (deltaRegistrationDecisionsResponse.IsSuccessStatusCode)
        {
            var jsonString = await deltaRegistrationDecisionsResponse.Content.ReadAsStringAsync();
            var serverCollection = JsonSerializer.Deserialize<AbstractCosmosSubmissionEvent[]>(jsonString, _jsonOptions);
            if (serverCollection.Length > 0)
            {
                results.AddRange(serverCollection);
            }
        }

        return results;
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

    public string GenerateReferenceNumber(CountryName countryName,
        RegistrationSubmissionType registrationSubmissionType, string organisationId, string twoDigitYear = null,
        MaterialType materialType = MaterialType.None)
    {
        if (string.IsNullOrEmpty(twoDigitYear))
        {
            twoDigitYear = (DateTime.Now.Year % 100).ToString("D2");
        }

        if (string.IsNullOrEmpty(organisationId))
        {
            throw new ArgumentNullException(nameof(organisationId));
        }

        var countryCode = ((char)countryName).ToString();

        var regType = ((char)registrationSubmissionType).ToString();

        var refNumber = $"R{twoDigitYear}{countryCode}{regType}{organisationId}{Generate4DigitNumber()}";

        if (registrationSubmissionType == RegistrationSubmissionType.Reprocessor ||
            registrationSubmissionType == RegistrationSubmissionType.Exporter)
        {
            refNumber = $"{refNumber}{materialType.GetDisplayName<MaterialType>()}";
        }

        return refNumber;
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

    private static string Generate4DigitNumber()
    {
        var min = 1000;
        var max = 10000;
        var randomNumber = RandomNumberGenerator.GetInt32(min, max);

        return randomNumber.ToString();
    }
}