using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Notify.Interfaces;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Organisations;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.Accounts
{

    [TestClass]
    public class MessagingServiceTests
    {
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<INotificationClient> _notificationClientMock = new();
        private readonly NullLogger<MessagingService> _nullLogger = new();
        private readonly Mock<IOptions<MessagingConfig>> _messagingConfigMock = new();
        private const string InvalidUserRecipient = "john.smith@gmail.com.com";
        private const string ApprovedUserRecipient = "john.smith@gmail.com";
        private const string ApprovedUserFirstName = "John";
        private const string ApprovedUserLastName = "Smith";

        private MessagingService _sut;

        [TestInitialize]
        public void Setup()
        {
            var messagingConfig = new MessagingConfig
            {
                OrganisationRegistrationSubmissionQueriedId = "queried-template-id",
                WelshOrganisationRegistrationSubmissionQueriedId = "welsh-queried-template-id",
                OrganisationRegistrationSubmissionDecisionId = "decision-template-id",
                WelshOrganisationRegistrationSubmissionDecisionId = "welsh-decision-template-id",
                OrganisationRegistrationResubmissionDecisionId = "resubmission-template-id",
                WelshOrganisationRegistrationResubmissionDecisionId = "welsh-resubmission-template-id"
            };

            _messagingConfigMock.Setup(m => m.Value).Returns(messagingConfig);
            _sut = new MessagingService(_notificationClientMock.Object, _messagingConfigMock.Object, _nullLogger);
        }

        [TestMethod]
        [DataRow("", "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "", "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", "")]
        [DataRow(null, "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", null, "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", null, "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", null, "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", null, "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", null)]
        [ExpectedException(typeof(ArgumentException))]
        public void ApprovedPersonAccepted_ArgumentException_Thrown_When_Parameters_Invalid(string email,
            string firstName, string lastName, string organisationName, string organisationNumber,
            string accountLoginUrl)
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                },
                OrganisationName = organisationName,
                OrganisationNumber = organisationNumber,
                AccountLoginUrl = accountLoginUrl,
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.ApprovedPersonAccepted(model);
        }

        [TestMethod]
        public void ApprovedPersonAccepted_Email_Sends_When_All_Parameters_Correct()
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = ApprovedUserRecipient,
                    FirstName = ApprovedUserFirstName,
                    LastName = ApprovedUserLastName
                },
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
            };

            var emailId = Guid.NewGuid().ToString();
            _notificationClientMock.Setup(x => x.SendEmail(
                    ApprovedUserRecipient,
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = emailId });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            string? approvedPersonAccepted = _sut.ApprovedPersonAccepted(model);

            // Assert
            _notificationClientMock.Verify(x => x.SendEmail(
                ApprovedUserRecipient,
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Once);

            approvedPersonAccepted.Should().Be(emailId);
        }

        [TestMethod]
        [DataRow("", "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "", "lastName", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough"),]
        [DataRow("bob@hotmail.com", "firstName", "", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", "", "Not good enough")]
        [DataRow(null, "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/", "")]
        [DataRow("bob@hotmail.com", null, "lastName", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", null, "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", null, "123456789", "https://www.gov.uk/",
            "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", null, "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", null, "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/", null)]
        [ExpectedException(typeof(ArgumentException))]
        public void ApprovedPersonRejected_ArgumentException_Thrown_When_Parameters_Invalid(string email,
            string firstName, string lastName, string organisationName, string organisationNumber,
            string accountLoginUrl, string rejectionComments)
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                },
                OrganisationName = organisationName,
                OrganisationNumber = organisationNumber,
                AccountLoginUrl = accountLoginUrl,
                RejectionComments = rejectionComments,
                DelegatedPeople = _fixture.CreateMany<UserEmailModel>(3).ToList()
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.ApprovedPersonRejected(model);
        }

        [TestMethod]
        public void ApprovedPersonRejected_Email_Sends_To_ApprovedUser_And_DelegatedUsers()
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = ApprovedUserRecipient,
                    FirstName = ApprovedUserFirstName,
                    LastName = ApprovedUserLastName
                },
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "Not good enough",
                DelegatedPeople = _fixture.CreateMany<UserEmailModel>(3).ToList()
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.ApprovedPersonRejected(model);

            // Assert
            emailIds.Count.Should().Be(4);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(4));
        }

        [TestMethod]
        [DataRow("", "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "", "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", "")]
        [DataRow(null, "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", null, "lastName", "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", null, "Org 1", "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", null, "123456789", "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", null, "https://www.gov.uk/")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", null)]
        [ExpectedException(typeof(ArgumentException))]
        public void DelegatedPersonAccepted_ArgumentException_Thrown_When_Parameters_Invalid(string email,
            string firstName,
            string lastName, string organisationName, string organisationNumber, string accountLoginUrl)
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel() { Email = email, FirstName = firstName, LastName = lastName },
                OrganisationName = organisationName,
                OrganisationNumber = organisationNumber,
                AccountLoginUrl = accountLoginUrl,
                DelegatedPeople = _fixture.CreateMany<UserEmailModel>(1).ToList()
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.DelegatedPersonAccepted(model);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void DelegatedPersonAccepted_Throws_Exception_When_More_Than_Once_Delegated_User()
        {
            // Arrange
            var applicationEmailModel = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                { Email = ApprovedUserRecipient, FirstName = ApprovedUserLastName, LastName = ApprovedUserLastName },
                OrganisationName = "Org 3",
                OrganisationNumber = "123456789",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                DelegatedPeople = _fixture.CreateMany<UserEmailModel>(2).ToList()
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.DelegatedPersonAccepted(applicationEmailModel);
        }

        [TestMethod]
        public void DelegatedPersonAccepted_Email_Sends_To_ApprovedUser_And_DelegatedUser()
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = ApprovedUserRecipient,
                    FirstName = ApprovedUserFirstName,
                    LastName = ApprovedUserLastName
                },
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "Not good enough",
                DelegatedPeople = _fixture.CreateMany<UserEmailModel>(1).ToList()
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.DelegatedPersonAccepted(model);

            // Assert
            emailIds.Count.Should().Be(2);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(2));
        }

        [TestMethod]
        [DataRow("", "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "", "lastName", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", "", "Not good enough")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/", "")]
        [DataRow(null, "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob1@hotmail.com", null, "lastName", "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob2@hotmail.com", "firstName", null, "Org 1", "123456789", "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob3@hotmail.com", "firstName", "lastName", null, "123456789", "https://www.gov.uk/",
            "Not good enough")]
        [DataRow("bob4@hotmail.com", "firstName", "lastName", "Org 1", null, "https://www.gov.uk/", "Not good enough")]
        [DataRow("bob5@hotmail.com", "firstName", "lastName", "Org 1", "123456789", null, "Not good enough")]
        [DataRow("bob6@hotmail.com", "firstName", "lastName", "Org 1", "123456789", "https://www.gov.uk/", null)]
        [ExpectedException(typeof(ArgumentException))]
        public void DelegatedPersonRejected_ArgumentException_Thrown_When_Parameters_Invalid(string email,
            string firstName, string lastName, string organisationName, string organisationNumber,
            string accountLoginUrl, string rejectionComments)
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                },
                OrganisationName = organisationName,
                OrganisationNumber = organisationNumber,
                AccountLoginUrl = accountLoginUrl,
                RejectionComments = rejectionComments,
                DelegatedPeople = _fixture.CreateMany<UserEmailModel>(1).ToList()
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.DelegatedPersonRejected(model);
        }

        [TestMethod]
        public void DelegatedPersonRejected_Email_Sends_To_ApprovedUser_And_DelegatedUser()
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = ApprovedUserRecipient,
                    FirstName = ApprovedUserFirstName,
                    LastName = ApprovedUserLastName
                },
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "Not good enough",
                DelegatedPeople = _fixture.CreateMany<UserEmailModel>(1).ToList(),
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.DelegatedPersonRejected(model);

            // Assert
            emailIds.Count.Should().Be(2);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(2));
        }

        [TestMethod]
        public void SubmissionAccepted_Email_Sends_To_All_Users_With_Approved_Enrolments()
        {
            // Arrange
            var model = new SubmissionEmailModel()
            {
                UserEmails = _fixture.CreateMany<UserEmailModel>(3).ToList(),
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                SubmissionPeriod = "Jan to July 2023",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.SubmissionAccepted(model, EventType.RegulatorPoMDecision);

            // Assert
            emailIds.Count.Should().Be(3);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(3));
        }

        [TestMethod]
        public void RegistrationAccepted_Email_Sends_To_All_Users_With_Approved_Enrolments()
        {
            // Arrange
            var model = new SubmissionEmailModel()
            {
                UserEmails = _fixture.CreateMany<UserEmailModel>(3).ToList(),
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                SubmissionPeriod = "Jan to July 2023",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.SubmissionAccepted(model, EventType.RegulatorRegistrationDecision);

            // Assert
            emailIds.Count.Should().Be(3);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(3));
        }


        [TestMethod]
        public void SubmissionRejected_Resubmission_Required_Email_Sends_To_All_Users_With_Approved_Enrolments()
        {
            // Arrange
            var model = new SubmissionEmailModel()
            {
                UserEmails = _fixture.CreateMany<UserEmailModel>(3).ToList(),
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                SubmissionPeriod = "Jan to July 2023",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "This was rejected as part of a test."
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.SubmissionRejected(model, true);

            // Assert
            emailIds.Count.Should().Be(3);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(3));
        }

        [TestMethod]
        public void RegistrationRejected_Email_Sends_To_All_Users_With_Approved_Enrolments()
        {
            // Arrange
            var model = new SubmissionEmailModel()
            {
                UserEmails = _fixture.CreateMany<UserEmailModel>(3).ToList(),
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                SubmissionPeriod = "Jan to July 2023",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "This was rejected as part of a test."
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.SubmissionRejected(model, null);

            // Assert
            emailIds.Count.Should().Be(3);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(3));
        }

        [TestMethod]
        public void SubmissionRejected_Resubmission_Not_Required_Email_Sends_To_All_Users_With_Approved_Enrolments()
        {
            // Arrange
            var model = new SubmissionEmailModel()
            {
                UserEmails = _fixture.CreateMany<UserEmailModel>(3).ToList(),
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                SubmissionPeriod = "Jan to July 2023",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "This was rejected as part of a test."
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = Guid.NewGuid().ToString() });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var emailIds = _sut.SubmissionRejected(model, false);

            // Assert
            emailIds.Count.Should().Be(3);

            _notificationClientMock.Verify(x => x.SendEmail(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Exactly(3));
        }

        [TestMethod]
        [DataRow("", "firstName", "lastName", "123456789", "DemotedDelegatedUsed")]
        [DataRow("bob@hotmail.com", "", "lastName", "123456789", "DemotedDelegatedUsed")]
        [DataRow("bob@hotmail.com", "firstName", "", "123456789", "DemotedDelegatedUsed")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "", "DemotedDelegatedUsed")]
        [ExpectedException(typeof(ArgumentException))]
        public void RemovedPerson_DelegatedPerson_ArgumentException_Thrown_When_Parameters_Invalid(
           string email,
           string firstName,
           string lastName,
           string organisationNumber,
           string type)
        {
            // Arrange
            var model = new AssociatedPersonResults
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                OrganisationId = organisationNumber,
                EmailNotificationType = type
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.SendRemovedApprovedPersonNotification(model, model.EmailNotificationType);
        }

        [TestMethod]
        [DataRow("", "firstName", "lastName", "123456789", "Org 1", "RemovedApprovedUser")]
        [DataRow("bob@hotmail.com", "", "lastName", "123456789", "Org 1", "RemovedApprovedUser")]
        [DataRow("bob@hotmail.com", "firstName", "", "123456789", "Org 1", "RemovedApprovedUser")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "123456789", "", "RemovedApprovedUser")]
        [DataRow("bob@hotmail.com", "firstName", "lastName", "", "Org 1", "RemovedApprovedUser")]
        [ExpectedException(typeof(ArgumentException))]
        public void RemovedPerson_ApprovedPerson_ArgumentException_Thrown_When_Parameters_Invalid(
            string email,
            string firstName,
            string lastName,
            string organisationNumber,
            string companyName,
            string type)
        {
            // Arrange
            var model = new AssociatedPersonResults
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CompanyName = companyName,
                OrganisationId = organisationNumber,
                EmailNotificationType = type,
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.SendRemovedApprovedPersonNotification(model, model.EmailNotificationType);
        }

        [TestMethod]
        public void RemoveApprovedPerson_Email_Sends_To_ApprovedUser()
        {
            var emailNotificationId = "C123456";
            // Arrange
            var model = new AssociatedPersonResults
            {
                Email = ApprovedUserRecipient,
                FirstName = ApprovedUserFirstName,
                LastName = ApprovedUserLastName,
                CompanyName = "Test Company",
                OrganisationId = "123987345",
                ServiceRoleId = 1,
                AccountSignInUrl = "https://www.gov.uk/report-data"
            };

            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null))
               .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = emailNotificationId });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var notificationId = _sut.SendRemovedApprovedPersonNotification(model, "RemovedApprovedUser");

            // Assert
            notificationId.Should().Be(emailNotificationId);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void RemoveApprovedPerson_Email_Sends_To_DemotedDelegatedUser()
        {
            var emailNotificationId = "P521254";
            // Arrange
            var model = new AssociatedPersonResults
            {
                Email = ApprovedUserRecipient,
                FirstName = ApprovedUserFirstName,
                LastName = ApprovedUserLastName,
                CompanyName = "Test Company",
                OrganisationId = "123987345",
                ServiceRoleId = 3,
            };

            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null))
               .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = emailNotificationId });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var notificationId = _sut.SendRemovedApprovedPersonNotification(model, "DemotedDelegatedUsed");

            // Assert
            notificationId.Should().Be(emailNotificationId);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void InviteNewApprovedPerson_ValidParameters_EmailSendsSuccessfully()
        {
            var emailNotificationId = "C123456";
            // Arrange
            var model = new AddRemoveNewApprovedPersonEmailModel
            {
                Email = "test@test.com",
                FirstName = "InvitedUserFirstName",
                LastName = "InvitedUserLastName",
                OrganisationNumber = "OrganisationNumber",
                InviteLink = "SomeInviteLink",
                CompanyName = "BlahCompanyName"
            };

            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null))
               .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = emailNotificationId });

            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    InviteNewApprovedPersonTemplateId = "SomeInviteNewApprovedPersonTemplateId"
                });

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            var notificationId = _sut.SendEmailToInvitedNewApprovedPerson(model);

            // Assert
            notificationId.Should().Be(emailNotificationId);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.Is<string>(x => x == model.Email),
               It.Is<string>(x => x == messagingConfig.Value.InviteNewApprovedPersonTemplateId),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionQueried_SendsEmail()
        {
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                Period = "2025"
            };


            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null));

            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    OrganisationRegistrationSubmissionQueriedId = "SomeInviteNewApprovedPersonTemplateId"
                });

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);
            _sut.OrganisationRegistrationSubmissionQueried(model);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.Is<string>(x => x == model.ToEmail),
               It.Is<string>(x => x == messagingConfig.Value.OrganisationRegistrationSubmissionQueriedId),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionQueried_SendsEmail_Throws_Exception()
        {
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                Period = "2025"
            };


            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null)).Throws<Exception>();

            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    OrganisationRegistrationSubmissionQueriedId = "SomeInviteNewApprovedPersonTemplateId"
                });

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);
            _sut.OrganisationRegistrationSubmissionQueried(model);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.Is<string>(x => x == model.ToEmail),
               It.Is<string>(x => x == messagingConfig.Value.OrganisationRegistrationSubmissionQueriedId),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionEmailModel_GetParameters()
        {
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                AgencyEmail = "test@test.com;test2@test.com",
                Period = "2025",
                IsWelsh = true,
            };

            var data = model.GetParameters;

            Assert.AreEqual("test@test.com", data["agency_email_welsh"]);
            Assert.AreEqual("test2@test.com", data["agency_email"]);
            Assert.AreEqual("Cyfoeth Naturiol Cymru (CNC)", data["agency_welsh"]);
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionRejected_SendsEmail()
        {
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                Period = "2025"
            };


            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null));

            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    OrganisationRegistrationSubmissionDecisionId = "SomeInviteNewApprovedPersonTemplateId"
                });

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);
            _sut.OrganisationRegistrationSubmissionDecision(model);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.Is<string>(x => x == model.ToEmail),
               It.Is<string>(x => x == messagingConfig.Value.OrganisationRegistrationSubmissionDecisionId),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionAccepted_SendsEmail()
        {
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                Period = "2025"
            };


            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null));

            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    OrganisationRegistrationSubmissionDecisionId = "SomeInviteNewApprovedPersonTemplateId"
                });

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);
            _sut.OrganisationRegistrationSubmissionDecision(model);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.Is<string>(x => x == model.ToEmail),
               It.Is<string>(x => x == messagingConfig.Value.OrganisationRegistrationSubmissionDecisionId),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionAccepted_SendsEmail_Throws_Exception()
        {
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                Period = "2025"
            };


            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null)).Throws<Exception>();

            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    OrganisationRegistrationSubmissionDecisionId = "SomeInviteNewApprovedPersonTemplateId"
                });

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);
            _sut.OrganisationRegistrationSubmissionDecision(model);

            _notificationClientMock.Verify(x => x.SendEmail(
               It.Is<string>(x => x == model.ToEmail),
               It.Is<string>(x => x == messagingConfig.Value.OrganisationRegistrationSubmissionDecisionId),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationResubmissionDecision_SendsEmail()
        {
            // Arrange
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                Period = "2025"
            };

            // Set up the mock for SendEmail
            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null));

            // Set up the messaging configuration
            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    OrganisationRegistrationResubmissionDecisionId = "SomeResubmissionTemplateId"
                });

            // Initialize the system under test (SUT)
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.OrganisationRegistrationResubmissionDecision(model);

            // Assert
            // Verify that SendEmail is called with the correct parameters
            _notificationClientMock.Verify(x => x.SendEmail(
               It.Is<string>(x => x == model.ToEmail),
               It.Is<string>(x => x == messagingConfig.Value.OrganisationRegistrationResubmissionDecisionId),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null), Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionQueried_Should_Call_SendRegistrationSubmissionEmail_With_Correct_TemplateId_English()
        {
            // Arrange
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                Period = "2025"
            };

            // Set up the mock for SendEmail
            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null));

            // Act
            _sut.OrganisationRegistrationSubmissionQueried(model);

            // Assert
            _notificationClientMock.Verify(nc => nc.SendEmail(
                It.Is<string>(x => x == model.ToEmail),
                (bool)model.IsWelsh ? "welsh-queried-template-id" : "queried-template-id",
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null),
                Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionQueried_Should_Call_SendRegistrationSubmissionEmail_With_Correct_TemplateId_Welsh()
        {
            // Arrange
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                AgencyEmail = "welsh@test.com;english@test.com",
                Period = "2025",
                IsWelsh = true
            };

            // Set up the mock for SendEmail
            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null));

            // Act
            _sut.OrganisationRegistrationSubmissionQueried(model);

            // Assert
            _notificationClientMock.Verify(nc => nc.SendEmail(
                It.Is<string>(x => x == model.ToEmail),
                "welsh-queried-template-id",
                It.Is<Dictionary<string, object>>(p =>
                    p["agency_welsh"].ToString() == "Cyfoeth Naturiol Cymru (CNC)" &&
                    p["agency_email_welsh"].ToString() == "welsh@test.com" &&
                    p["agency_email"].ToString() == "english@test.com"),
                null,
                null,
                null),
                Times.Exactly(1));
        }

        [TestMethod]
        public void OrganisationRegistrationSubmissionDecision_Should_Call_SendRegistrationSubmissionEmail_With_Correct_TemplateId()
        {
            // Arrange
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                AgencyEmail = "agency@test.com",
                Period = "2025",
                IsWelsh = false
            };

            // Act
            _sut.OrganisationRegistrationSubmissionDecision(model);

            // Assert
            _notificationClientMock.Verify(nc => nc.SendEmail(
                model.ToEmail,
                "decision-template-id",
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null),
                Times.Once);
        }

        [TestMethod]
        public void OrganisationRegistrationResubmissionDecision_Should_Call_SendRegistrationSubmissionEmail_With_Correct_TemplateId()
        {
            // Arrange
            var model = new OrganisationRegistrationSubmissionEmailModel
            {
                ToEmail = "test@test.com",
                OrganisationName = "org name",
                OrganisationNumber = "12345",
                Agency = "Agency",
                AgencyEmail = "agency@test.com",
                Period = "2025",
                IsWelsh = false
            };

            // Act
            _sut.OrganisationRegistrationResubmissionDecision(model);

            // Assert
            _notificationClientMock.Verify(nc => nc.SendEmail(
                model.ToEmail,
                "resubmission-template-id",
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null),
                Times.Once);
        }

        [TestMethod]
        [DataRow("", "InvitedUserFirstName",
             "InvitedUserLastName", "OrganisationNumber", "SomeInviteLink", "CompanyName")]
        [DataRow("test@test.com", "",
              "InvitedUserLastName", "OrganisationNumber", "SomeInviteLink", "CompanyName")]
        [DataRow("test@test.com", "InvitedUserFirstName",
              "", "OrganisationNumber", "SomeInviteLink", "CompanyName")]
        [DataRow("test@test.com", "InvitedUserFirstName",
              "InvitedUserLastName", "", "SomeInviteLink", "CompanyName")]
        [DataRow("test@test.com", "InvitedUserFirstName",
              "InvitedUserLastName", "OrganisationNumber", "", "CompanyName")]
        [DataRow("test@test.com", "InvitedUserFirstName",
             "InvitedUserLastName", "OrganisationNumber", "SomeInviteLink", "")]
        public void InviteNewApprovedPerson_InvalidParameters_ShouldThrowArgumentException(
             string email, string firstName,
             string lastName, string organisationNumber,
             string inviteLink, string companyName)

        {
            var emailNotificationId = "C123456";
            // Arrange
            var model = new AddRemoveNewApprovedPersonEmailModel
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                OrganisationNumber = organisationNumber,
                InviteLink = inviteLink,
                CompanyName = companyName
            };

            _notificationClientMock.Setup(x => x.SendEmail(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<Dictionary<string, object>>(),
               null,
               null,
               null))
               .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = emailNotificationId });

            var messagingConfig = Options.Create(
                new MessagingConfig
                {
                    InviteNewApprovedPersonTemplateId = "SomeInviteNewApprovedPersonTemplateId"
                });

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            Assert.ThrowsException<ArgumentException>(() => _sut.SendEmailToInvitedNewApprovedPerson(model));
        }


        [TestMethod]
        public void Incorrect_Email_Causes_Exception()
        {
            // Arrange
            var model = new ApplicationEmailModel
            {
                ApprovedPerson = new UserEmailModel()
                {
                    Email = InvalidUserRecipient,
                    FirstName = ApprovedUserFirstName,
                    LastName = ApprovedUserLastName
                },
                OrganisationName = "Wallace's Company",
                OrganisationNumber = "123987345",
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
            };

            var emailId = Guid.NewGuid().ToString();
            _notificationClientMock.Setup(x => x.SendEmail(
                    ApprovedUserRecipient,
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() { id = emailId });

            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            string? approvedPersonAccepted = _sut.ApprovedPersonAccepted(model);

            // Assert
            _notificationClientMock.Verify(x => x.SendEmail(
                InvalidUserRecipient,
                It.IsAny<string>(),
                It.IsAny<Dictionary<string, object>>(),
                null,
                null,
                null), Times.Once);

            approvedPersonAccepted.Should().Be(null);
        }
    }
}