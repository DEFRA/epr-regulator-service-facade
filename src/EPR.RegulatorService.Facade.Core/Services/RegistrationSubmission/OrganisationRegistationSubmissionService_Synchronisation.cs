using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations.CommonData.SubmissionDetails;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using static EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission.OrganisationRegistrationSubmissionService;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission
{
    public partial class OrganisationRegistrationSubmissionService
    {
        // 14 is used to support existing data and will be removed for R9.0
        private const int _producerApplicationRefNumLength = 14;

        public string GenerateReferenceNumber(CountryName countryName,
                                              RegistrationSubmissionType registrationSubmissionType,
                                              string applicationReferenceNumber,
                                              string organisationId,
                                              string twoDigitYear = null,
                                              MaterialType materialType = MaterialType.None)
        {
            string refNumber;

            if (string.IsNullOrWhiteSpace(twoDigitYear))
            {
                throw new ArgumentNullException(nameof(twoDigitYear));
            }

            if (string.IsNullOrWhiteSpace(organisationId))
            {
                throw new ArgumentNullException(nameof(organisationId));
            }

            var countryCode = ((char)countryName).ToString();

            var regType = ((char)registrationSubmissionType).ToString();

            if (registrationSubmissionType == RegistrationSubmissionType.ComplianceScheme &&
                applicationReferenceNumber.Length > _producerApplicationRefNumLength)
            {
                refNumber = $"R{twoDigitYear}{countryCode}{regType}{organisationId}{ExtractUniqueNumberFromAppRefNumber(applicationReferenceNumber)}{GenerateXDigitNumber(100, 1000)}";
            }
            else
            {
                refNumber = $"R{twoDigitYear}{countryCode}{regType}{organisationId}{GenerateXDigitNumber(1000, 10000)}";
            }

            if (registrationSubmissionType == RegistrationSubmissionType.Reprocessor ||
                registrationSubmissionType == RegistrationSubmissionType.Exporter)
            {
                refNumber = $"{refNumber}{materialType.GetDisplayName<MaterialType>()}";
            }

            return refNumber;
        }

        private static int GenerateXDigitNumber(int min, int max)
        {
            var randomNumber = RandomNumberGenerator.GetInt32(min, max);

            return randomNumber;
        }

        private static string ExtractUniqueNumberFromAppRefNumber(string appRefNumber)
        {
            // PEPR000001XXX25C1 is the agreed format of the reference number hence we are targetting the 10th index as per the requirements
            return appRefNumber.Substring(10, 3);
        }

        public static void MergeCosmosUpdates(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> requestedList)
        {
            foreach (var item in requestedList.items)
            {
                var cosmosItems = deltaRegistrationDecisionsResponse.Where(x => !string.IsNullOrWhiteSpace(x.AppReferenceNumber)
                                  && x.AppReferenceNumber.Equals(item?.ApplicationReferenceNumber, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Created);
                var regulatorDecisions = cosmosItems.Where(x => x.Type.Equals("RegulatorRegistrationDecision", StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Created).ToList();

                ProcessRegulatorDecisions(item, regulatorDecisions);
            }
        }

        public static void MergeCosmosUpdates(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, OrganisationRegistrationSubmissionDetailsResponse item)
        {
            var cosmosItems = deltaRegistrationDecisionsResponse.Where(x => !string.IsNullOrWhiteSpace(x.AppReferenceNumber) && x.AppReferenceNumber.Equals(item.ApplicationReferenceNumber, StringComparison.OrdinalIgnoreCase))
                                                                .OrderBy(x => x.Created);
            var regulatorDecisions = cosmosItems.Where(x => x.Type.Equals("RegulatorRegistrationDecision", StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.Created );

            foreach (var decision in regulatorDecisions)
            {
                AssignRegulatorDetails(item, decision);
            }
        }

        private static void ProcessRegulatorDecisions(
            OrganisationRegistrationSubmissionSummaryResponse item,
            List<AbstractCosmosSubmissionEvent> regulatorDecisions)
        {
            foreach (var cosmosItem in regulatorDecisions)
            {
                if (item.RegulatorDecisionDate is null || cosmosItem.Created > item.RegulatorDecisionDate)
                {
                    if (item.IsResubmission && "Granted Refused".Contains(cosmosItem.Decision))
                    {
                        ProcessResubmissionDecision(item, cosmosItem);
                    }
                    else
                    {
                        ProcessStandardDecision(item, cosmosItem);
                    }
                }
            }
        }
        private static void ProcessResubmissionDecision(
            OrganisationRegistrationSubmissionSummaryResponse item,AbstractCosmosSubmissionEvent cosmosItem)
        {
            var resubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
            item.ResubmissionStatus = resubmissionStatus switch
            {
                RegistrationSubmissionStatus.Granted => RegistrationSubmissionStatus.Accepted,
                RegistrationSubmissionStatus.Refused => RegistrationSubmissionStatus.Rejected,
                _ => item.ResubmissionStatus
            };

            item.RegulatorResubmissionDecisionDate = cosmosItem.Created;
        }

        private static void ProcessStandardDecision(
            OrganisationRegistrationSubmissionSummaryResponse item,
            AbstractCosmosSubmissionEvent cosmosItem)
        {
            item.SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
            item.RegulatorDecisionDate = cosmosItem.Created;
            item.StatusPendingDate = cosmosItem.DecisionDate;
            if (!string.IsNullOrWhiteSpace(cosmosItem.RegistrationReferenceNumber))
            {
                item.RegistrationReferenceNumber = cosmosItem.RegistrationReferenceNumber;
            }
            if (cosmosItem.Decision == "Granted")
            {
                item.RegistrationDate = cosmosItem.Created;
            }
        }

        private static void AssignRegulatorDetails(OrganisationRegistrationSubmissionDetailsResponse item, AbstractCosmosSubmissionEvent? cosmosItem)
        {
            if (item.RegulatorDecisionDate is null || cosmosItem.Created >= item.RegulatorDecisionDate)
            {
                item.RegulatorComments = cosmosItem.Comments;

                if (item.IsResubmission && "Granted Refused".Contains(cosmosItem.Decision))
                {
                    //To avoid checking magic strings, assign the decision first & check on the enum
                    //and then re-assign as resubmission uses different status
                    item.RegulatorResubmissionDecisionDate = cosmosItem.Created;
                    item.SubmissionDetails.ResubmissionDecisionDate = cosmosItem.Created;
                    var resubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
                    SetResubmissionStatus(item, resubmissionStatus);
                }
                else
                {
                    item.SubmissionDetails.DecisionDate = cosmosItem.DecisionDate ?? cosmosItem.Created;
                    item.SubmissionStatus = Enum.Parse<RegistrationSubmissionStatus>(cosmosItem.Decision);
                    item.StatusPendingDate = cosmosItem.DecisionDate;
                    item.RegistrationReferenceNumber = string.IsNullOrWhiteSpace(cosmosItem.RegistrationReferenceNumber) ? item.RegistrationReferenceNumber : cosmosItem.RegistrationReferenceNumber;
                    item.SubmissionDetails.Status = item.SubmissionStatus;
                    item.RegulatorDecisionDate = cosmosItem.Created;

                    if (cosmosItem.Decision == "Granted")
                    {
                        item.RegistrationDate = item.SubmissionDetails.RegistrationDate = cosmosItem.Created;
                    }
                }
            }
            else
            {
                item.RegulatorComments += $"<br/>{cosmosItem.Comments}";
            }
        }

        private static void SetResubmissionStatus(OrganisationRegistrationSubmissionDetailsResponse item, RegistrationSubmissionStatus resubmissionStatus)
        {
            if (resubmissionStatus == RegistrationSubmissionStatus.Granted)
            {
                item.ResubmissionStatus = RegistrationSubmissionStatus.Accepted;
                item.SubmissionDetails.ResubmissionStatus = item.ResubmissionStatus.ToString();
            }
            else if (resubmissionStatus == RegistrationSubmissionStatus.Refused)
            {
                item.ResubmissionStatus = RegistrationSubmissionStatus.Rejected;
                item.SubmissionDetails.ResubmissionStatus = item.ResubmissionStatus.ToString();
            }
            else
            {
                item.SubmissionDetails.Status = item.SubmissionStatus = resubmissionStatus;
            }
        }

        public static void ApplyAppRefNumbersForRequiredStatuses(List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisionsResponse, string statuses, GetOrganisationRegistrationSubmissionsFilter filter)
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

            public string FileId { get; set; }
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
