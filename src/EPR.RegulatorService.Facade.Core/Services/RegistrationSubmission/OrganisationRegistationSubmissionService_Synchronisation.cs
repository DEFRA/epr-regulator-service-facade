using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission
{
    public partial class OrganisationRegistrationSubmissionService
    {
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

        private static string Generate4DigitNumber()
        {
            var min = 1000;
            var max = 10000;
            var randomNumber = RandomNumberGenerator.GetInt32(min, max);

            return randomNumber.ToString();
        }

        private static void MergeCosmosUpdates(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> requestedList)
        {
            foreach (var item in requestedList.items)
            {
                var cosmosItems = deltaRegistrationDecisionsResponse.Where(x => !string.IsNullOrWhiteSpace(x.AppReferenceNumber)
                                  && x.AppReferenceNumber.Equals(item?.ApplicationReferenceNumber, StringComparison.OrdinalIgnoreCase));
                var regulatorDecisions = cosmosItems.Where(x => x.Type.Equals("RegulatorRegistrationDecision", StringComparison.OrdinalIgnoreCase)).ToList();
                var producerComments = cosmosItems.Where(x => x.Type.Equals("RegistrationApplicationSubmitted", StringComparison.OrdinalIgnoreCase)).Select(x => x.Created );

                foreach (var cosmosItem in regulatorDecisions)
                {
                    if (item.RegulatorCommentDate is null || cosmosItem.Created > item.RegulatorCommentDate)
                    {
                        item.RegulatorCommentDate = cosmosItem.Created;
                        item.RegistrationReferenceNumber = cosmosItem.RegistrationReferenceNumber ?? item.RegistrationReferenceNumber;
                        item.StatusPendingDate = cosmosItem.DecisionDate;
                        item.SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
                    }
                }
                foreach (var cosmosDate in producerComments)
                {
                    item.ProducerCommentDate = cosmosDate;
                    if (item.RegulatorCommentDate is null || cosmosDate > item.RegulatorCommentDate)
                    {
                        item.SubmissionStatus = RegistrationSubmissionStatus.Updated;
                    }
                }
            }
        }

        private static void MergeCosmosUpdates(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, RegistrationSubmissionOrganisationDetailsResponse item)
        {
            var cosmosItems = deltaRegistrationDecisionsResponse.Where(x => !string.IsNullOrWhiteSpace(x.AppReferenceNumber) && x.AppReferenceNumber.Equals(item.ApplicationReferenceNumber, StringComparison.OrdinalIgnoreCase));

            foreach (var cosmosItem in cosmosItems)
            {
                if (cosmosItem.Type.Equals("RegulatorRegistrationDecision", StringComparison.OrdinalIgnoreCase))
                {
                    AssignRegulatorDetails(item, cosmosItem);
                }
                else if (cosmosItem.Type.Equals("RegistrationApplicationSubmitted", StringComparison.OrdinalIgnoreCase))
                {
                    AssignProducerDetails(item, cosmosItem);
                }
            }

            if ( item.ProducerCommentDate is not null && item.RegulatorDecisionDate is not null && item.ProducerCommentDate > item.RegulatorDecisionDate)
            {
                item.SubmissionStatus = RegistrationSubmissionStatus.Updated;
                item.SubmissionDetails.Status = item.SubmissionStatus;
            }
        }

        private static void AssignProducerDetails(RegistrationSubmissionOrganisationDetailsResponse item, AbstractCosmosSubmissionEvent? cosmosItem)
        {
            if (item.ProducerCommentDate is null || cosmosItem.Created >= item.ProducerCommentDate)
            {
                item.ProducerComments = cosmosItem.Comments;
                item.ProducerCommentDate = cosmosItem.Created;
            }
            else
            {
                item.ProducerComments += $"<br/>{cosmosItem.Comments}";
            }

            if (item.RegulatorDecisionDate is null || cosmosItem.Created >= item.RegulatorDecisionDate)
            {
                item.SubmissionStatus = RegistrationSubmissionStatus.Updated;
                item.SubmissionDetails.Status = item.SubmissionStatus;
            }
        }

        private static void AssignRegulatorDetails(RegistrationSubmissionOrganisationDetailsResponse item, AbstractCosmosSubmissionEvent? cosmosItem)
        {
            if (item.RegulatorDecisionDate is null || cosmosItem.Created >= item.RegulatorDecisionDate)
            {
                item.RegulatorComments = cosmosItem.Comments;
                item.RegulatorDecisionDate = cosmosItem.Created;
                item.StatusPendingDate = cosmosItem.DecisionDate;
                item.SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
                item.SubmissionDetails.Status = item.SubmissionStatus;
                item.SubmissionDetails.DecisionDate = cosmosItem.DecisionDate ?? cosmosItem.Created;
                item.RegistrationReferenceNumber = cosmosItem.RegistrationReferenceNumber;
            }
            else
            {
                item.RegulatorComments += $"<br/>{cosmosItem.Comments}";
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

        [DebuggerDisplay("{Type},{AppReferenceNumber},{Created},{Decision},{DecisionDate}")]
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

    }
}
