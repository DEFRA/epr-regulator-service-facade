using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
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
            var filter = new GetOrganisationRegistrationSubmissionsFilter();
            var statuses = "Pending";

            // Act
            ApplyAppRefNumbersForRequiredStatuses(events, statuses, filter);

            // Assert
            // Includes duplicates since no Distinct is used
            Assert.AreEqual("APP013 APP013 APP014", filter.ApplicationReferenceNumbers);
        }
    }
}