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
        private const string ApprovedUserRecipient = "john.smith@gmail.com";
        private const string ApprovedUserFirstName = "John";
        private const string ApprovedUserLastName = "Smith";

        private MessagingService _sut;

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
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = emailId});

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
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                ApprovedPerson = new UserEmailModel() {Email = email, FirstName = firstName, LastName = lastName},
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
                    {Email = ApprovedUserRecipient, FirstName = ApprovedUserLastName, LastName = ApprovedUserLastName},
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
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "This was rejected as part of a test."
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "This was rejected as part of a test."
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                AccountLoginUrl = "http://www.gov.uk/guidance/report-packaging-data",
                RejectionComments = "This was rejected as part of a test."
            };

            _notificationClientMock.Setup(x => x.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    null,
                    null))
                .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = Guid.NewGuid().ToString()});

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
                null), Times.Exactly(3));
        }
        
         [TestMethod]
         [DataRow("", "firstName", "lastName", "123456789", 3)]
         [DataRow("bob@hotmail.com", "", "lastName", "123456789", 3)]
         [DataRow("bob@hotmail.com", "firstName", "","123456789", 3)]
         [DataRow("bob@hotmail.com", "firstName", "lastName","", 3)]
         [ExpectedException(typeof(ArgumentException))]
        public void RemovedPerson_DelegatedPerson_ArgumentException_Thrown_When_Parameters_Invalid(
            string email,
            string firstName, 
            string lastName, 
            string organisationNumber,
            int serviceRoleId)
        {
            // Arrange
            var model = new AssociatedPersonResults
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                OrganisationId = organisationNumber,
                ServiceRoleId = serviceRoleId,
            };

            var messagingConfig = Options.Create(new MessagingConfig());
            _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

            // Act
            _sut.SendRemovedApprovedPersonNotification(model, serviceRoleId );
        }
         
         [TestMethod]
         [DataRow("", "firstName", "lastName", "123456789", "Org 1", 1)]
         [DataRow("bob@hotmail.com", "", "lastName", "123456789","Org 1", 1)]
         [DataRow("bob@hotmail.com", "firstName", "", "123456789","Org 1", 1)]
         [DataRow("bob@hotmail.com", "firstName", "lastName",  "123456789","", 1)]
         [DataRow("bob@hotmail.com", "firstName", "lastName", "","Org 1", 1)]
         [ExpectedException(typeof(ArgumentException))]
         public void RemovedPerson_ApprovedPerson_ArgumentException_Thrown_When_Parameters_Invalid(
             string email,
             string firstName, 
             string lastName, 
             string organisationNumber,
             string companyName,
             int serviceRoleId)
         {
             // Arrange
             var model = new AssociatedPersonResults
             {
                 Email = email,
                 FirstName = firstName,
                 LastName = lastName,
                 CompanyName = companyName,
                 OrganisationId = organisationNumber,
                 ServiceRoleId = serviceRoleId,
             };

             var messagingConfig = Options.Create(new MessagingConfig());
             _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

             // Act
             _sut.SendRemovedApprovedPersonNotification(model, serviceRoleId );
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
             };

             _notificationClientMock.Setup(x => x.SendEmail(
                     It.IsAny<string>(),
                     It.IsAny<string>(),
                     It.IsAny<Dictionary<string, object>>(),
                     null,
                     null))
                 .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = emailNotificationId});

             var messagingConfig = Options.Create(new MessagingConfig());

             _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

             // Act
             var notificationId = _sut.SendRemovedApprovedPersonNotification(model, 1);

             // Assert
             notificationId.Should().Be(emailNotificationId);

             _notificationClientMock.Verify(x => x.SendEmail(
                 It.IsAny<string>(),
                 It.IsAny<string>(),
                 It.IsAny<Dictionary<string, object>>(),
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
                     null))
                 .Returns(new Notify.Models.Responses.EmailNotificationResponse() {id = emailNotificationId});

             var messagingConfig = Options.Create(new MessagingConfig());

             _sut = new MessagingService(_notificationClientMock.Object, messagingConfig, _nullLogger);

             // Act
             var notificationId = _sut.SendRemovedApprovedPersonNotification(model, 3);

             // Assert
             notificationId.Should().Be(emailNotificationId);

             _notificationClientMock.Verify(x => x.SendEmail(
                 It.IsAny<string>(),
                 It.IsAny<string>(),
                 It.IsAny<Dictionary<string, object>>(),
                 null,
                 null), Times.Exactly(1));
         }
    }
    
    
}