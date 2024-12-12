using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission.OrganisationRegistrationSubmissionService;

namespace EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission.Tests
{
    [TestClass()]
    public class ApplyAppRefNumbersForRequiredStatusesTests
    {
        [TestMethod]
        public void NoStatuses_EmptyResult()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Pending", AppReferenceNumber = "APP001" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = ""; // no statuses

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            Assert.AreEqual(string.Empty, filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void WhitespaceOnlyStatuses_EmptyResult()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Granted", AppReferenceNumber = "APP002" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "   "; // whitespace only

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            Assert.AreEqual(string.Empty, filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void NoMatchingDecision_EmptyResult()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Granted", AppReferenceNumber = "APP003" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "Refused"; // no event with Decision = Refused

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            Assert.AreEqual(string.Empty, filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void SingleStatus_SingleMatch_ReturnsAppRefNumber()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Pending", AppReferenceNumber = "APP004" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "Pending";

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            Assert.AreEqual("APP004", filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void SingleStatus_MultipleMatches_ReturnsAllAppRefNumbers()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Pending", AppReferenceNumber = "APP005" },
                new() { Decision = "Pending", AppReferenceNumber = "APP006" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "Pending";

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            // Order is based on LINQ order - we assume stable ordering from original list
            Assert.AreEqual("APP005 APP006", filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void MultipleStatuses_WithMatches_ReturnsCombinedResults()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Pending", AppReferenceNumber = "APP007" },
                new() { Decision = "Granted", AppReferenceNumber = "APP008" },
                new() { Decision = "Granted", AppReferenceNumber = "APP009" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "Pending Granted";

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            // Should include APP007 (Pending), APP008 and APP009 (Granted)
            Assert.AreEqual("APP007 APP008 APP009", filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void MultipleStatuses_ExtraSpacesStillWorks()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Refused", AppReferenceNumber = "APP010" },
                new() { Decision = "Cancelled", AppReferenceNumber = "APP011" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "  Refused   Cancelled  "; // extra spaces

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            Assert.AreEqual("APP010 APP011", filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void CaseSensitive_Mismatch_NoResults()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "PENDING", AppReferenceNumber = "APP012" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "Pending"; // differs in case

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            // Since 'PENDING' != 'Pending' (no case-insensitive logic), no match
            Assert.AreEqual(string.Empty, filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void Duplicates_AllIncluded()
        {
            // Arrange
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { Decision = "Pending", AppReferenceNumber = "APP013" },
                new() { Decision = "Pending", AppReferenceNumber = "APP013" }, // duplicate AppRef
                new() { Decision = "Pending", AppReferenceNumber = "APP014" }
            };
            var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();
            var statuses = "Pending";

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            // Includes duplicates since no Distinct is used
            Assert.AreEqual("APP013 APP013 APP014", filter.ApplicationReferenceNumbers);
        }

        [TestMethod]
        public void ProducerCommentDateNull_RegulatorDecisionDateNull_SetsCommentsAndUpdated()
        {
            // Arrange
            var item = CreateDefaultItem();
            item.ProducerCommentDate = null;
            item.RegulatorDecisionDate = null;
            item.ProducerComments = "Old comment";

            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = DateTime.UtcNow,
                Comments = "New comment",
            };

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            Assert.AreEqual("New comment", item.ProducerComments, "Should overwrite ProducerComments");
            Assert.AreEqual(cosmosItem.Created, item.ProducerCommentDate);
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionStatus, "Should be updated since RegulatorDecisionDate is null.");
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void ProducerCommentDateNull_RegulatorDecisionDateLater_NoUpdate()
        {
            // Arrange
            var item = CreateDefaultItem();
            item.ProducerCommentDate = null;
            item.RegulatorDecisionDate = DateTime.UtcNow.AddHours(1); // Future date
            item.SubmissionStatus = RegistrationSubmissionStatus.Pending;
            item.SubmissionDetails.Status = RegistrationSubmissionStatus.Pending;
            item.ProducerComments = "Old comment";

            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = DateTime.UtcNow,
                Comments = "New comment"
            };

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            Assert.AreEqual("New comment", item.ProducerComments);
            Assert.AreEqual(cosmosItem.Created, item.ProducerCommentDate);
            // Since cosmosItem.Created < RegulatorDecisionDate (Regulator date is in the future)
            // Actually, cosmosItem.Created < RegulatorDecisionDate means no update:
            // Condition: (RegulatorDecisionDate is null OR Created >= RegulatorDecisionDate)
            // cosmosItem.Created < RegulatorDecisionDate means NOT updated.
            Assert.AreEqual(RegistrationSubmissionStatus.Pending, item.SubmissionStatus, "No update if Created < RegulatorDecisionDate.");
            Assert.AreEqual(RegistrationSubmissionStatus.Pending, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void ProducerCommentDateSet_CosmosItemOlder_AppendsCommentNoStatusUpdate()
        {
            // Arrange
            var item = CreateDefaultItem();
            var currentTime = DateTime.UtcNow;
            item.ProducerCommentDate = currentTime; // Existing producer comment date
            item.ProducerComments = "Existing comment";
            item.RegulatorDecisionDate = currentTime; // Regulator decision date is newer than cosmosItem
            item.SubmissionStatus = RegistrationSubmissionStatus.Granted; // Some initial status
            item.SubmissionDetails.Status = RegistrationSubmissionStatus.Granted;
            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = currentTime.AddMinutes(-10), // Older than ProducerCommentDate
                Comments = "Older Cosmos Comment"
            };

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            // The else branch should have appended the comment
            Assert.AreEqual("Existing comment<br/>Older Cosmos Comment", item.ProducerComments, "Should append the new comment with <br/>");
            Assert.AreEqual(currentTime, item.ProducerCommentDate, "ProducerCommentDate should remain unchanged since cosmosItem is older.");
            // Since cosmosItem.Created < RegulatorDecisionDate, no update to SubmissionStatus
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus, "No status update expected.");
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionDetails.Status, "SubmissionDetails should remain unchanged.");
        }

        [TestMethod]
        public void ProducerCommentDateSet_CosmosItemOlder_AppendsCommentNoUpdate()
        {
            // Arrange
            var item = CreateDefaultItem();
            var originalDate = DateTime.UtcNow.AddHours(-2);
            var producerDate = DateTime.UtcNow.AddHours(-1);
            item.ProducerCommentDate = producerDate;
            item.ProducerComments = "Existing comment";
            item.RegulatorDecisionDate = DateTime.UtcNow;
            item.SubmissionStatus = RegistrationSubmissionStatus.Granted;
            item.SubmissionDetails.Status = RegistrationSubmissionStatus.Granted;

            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = originalDate, // older than ProducerCommentDate
                Comments = "Old Cosmos Comment"
            };

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            // Should append due to older date
            Assert.AreEqual("Existing comment<br/>Old Cosmos Comment", item.ProducerComments);
            // ProducerCommentDate remains the newer one.
            Assert.AreEqual(producerDate, item.ProducerCommentDate);
            // cosmosItem.Created < RegulatorDecisionDate, no update.
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void ProducerCommentDateSet_CosmosItemNewer_OverwritesCommentsAndUpdatesStatusIfAllowed()
        {
            // Arrange
            var item = CreateDefaultItem();
            var olderProducerDate = DateTime.UtcNow.AddHours(-1);
            item.ProducerCommentDate = olderProducerDate;
            item.ProducerComments = "Older comment";
            item.RegulatorDecisionDate = DateTime.UtcNow.AddHours(-2); // older decision date
            item.SubmissionStatus = RegistrationSubmissionStatus.Pending;
            item.SubmissionDetails.Status = RegistrationSubmissionStatus.Pending;

            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = DateTime.UtcNow, // newer than ProducerCommentDate
                Comments = "New Cosmos Comment"
            };

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            // Should overwrite since cosmosItem.Created >= ProducerCommentDate
            Assert.AreEqual("New Cosmos Comment", item.ProducerComments);
            Assert.AreEqual(cosmosItem.Created, item.ProducerCommentDate);

            // RegulatorDecisionDate is older than cosmosItem.Created, so condition holds true and updates status
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void RegulatorDecisionDateNullWithOlderCommentStillUpdates()
        {
            // Arrange
            var item = CreateDefaultItem();
            item.ProducerCommentDate = DateTime.UtcNow;
            item.ProducerComments = "Existing comment";
            item.RegulatorDecisionDate = null; // always updates status
            item.SubmissionStatus = RegistrationSubmissionStatus.Pending;

            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = DateTime.UtcNow.AddMinutes(-10),
                Comments = "Older Cosmos Comment"
            };

            // cosmosItem.Created < ProducerCommentDate means append comment
            // Also, RegulatorDecisionDate null means always updated.

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            Assert.AreEqual("Existing comment<br/>Older Cosmos Comment", item.ProducerComments);
            // ProducerCommentDate remains unchanged since cosmosItem is older
            // item.ProducerCommentDate stays the same
            // RegulatorDecisionDate null means always updated
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void CosmosItemEqualToProducerCommentDate_OverwritesComments()
        {
            // Arrange
            var item = CreateDefaultItem();
            var exactTime = DateTime.UtcNow;
            item.ProducerCommentDate = exactTime;
            item.ProducerComments = "Old comment";
            item.RegulatorDecisionDate = DateTime.UtcNow.AddHours(-1);
            item.SubmissionStatus = RegistrationSubmissionStatus.Pending;
            item.SubmissionDetails.Status = RegistrationSubmissionStatus.Pending;

            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = exactTime, // equal to ProducerCommentDate
                Comments = "Equal Time Comment"
            };

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            // If cosmosItem.Created >= ProducerCommentDate (equal counts as >=)
            // Overwrites producer comments
            Assert.AreEqual("Equal Time Comment", item.ProducerComments);
            Assert.AreEqual(exactTime, item.ProducerCommentDate);
            // cosmosItem.Created >= RegulatorDecisionDate, sets to Updated
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Updated, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void CosmosItemCreatedBeforeRegulatorDecisionDate_NoUpdate()
        {
            // Arrange
            var item = CreateDefaultItem();
            var regulatorDate = DateTime.UtcNow;
            item.RegulatorDecisionDate = regulatorDate;
            item.ProducerCommentDate = null;
            item.ProducerComments = "Initial comment";
            item.SubmissionStatus = RegistrationSubmissionStatus.Queried;
            item.SubmissionDetails.Status = RegistrationSubmissionStatus.Queried;

            var cosmosItem = new AbstractCosmosSubmissionEvent
            {
                Created = regulatorDate.AddMinutes(-10), // earlier than RegulatorDecisionDate
                Comments = "Older comment"
            };

            // Act
            AssignProducerDetails(item, cosmosItem);

            // Assert
            // ProducerCommentDate null or older means overwrite
            Assert.AreEqual("Older comment", item.ProducerComments);
            Assert.AreEqual(cosmosItem.Created, item.ProducerCommentDate);
            // Created < RegulatorDecisionDate means no updated
            Assert.AreEqual(RegistrationSubmissionStatus.Queried, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Queried, item.SubmissionDetails.Status);
        }
        
        private RegistrationSubmissionOrganisationDetailsResponse CreateDefaultItem()
        {
            return new RegistrationSubmissionOrganisationDetailsResponse
            {
                SubmissionDetails = new RegistrationSubmissionOrganisationSubmissionSummaryDetails
                {
                    Status = RegistrationSubmissionStatus.None
                },
                SubmissionStatus = RegistrationSubmissionStatus.None
            };
        }
    }
}