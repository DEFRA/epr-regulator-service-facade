using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Accounts;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.Accounts
{
    [TestClass]
    public class ApplicationControllerTests
    {
        private readonly NullLogger<ApplicationController> _nullLogger = new();
        private readonly Mock<IMessagingService> _messagingServiceMock = new();
        private readonly ApplicationController _sut;

        public ApplicationControllerTests()
        {
            var messagingConfig = Options.Create(new MessagingConfig());

            _sut = new ApplicationController(_nullLogger, messagingConfig, _messagingServiceMock.Object);
        }

        [TestMethod]
        public async Task AccountsController_ShouldReturn200_WhenSuccess()
        {
            // Arrange
            var govNotificationRequestModel = GetGovNotificationRequestModel();
            var applicationEmailModel = GetApplicationEmailModel();

            _messagingServiceMock.Setup(x => x.ApprovedPersonAccepted(applicationEmailModel));

            // Act
            var govNotification = await _sut.GovNotification(govNotificationRequestModel);

            var result = govNotification as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(200);
            _messagingServiceMock.Verify(x => x.ApprovedPersonAccepted(It.IsAny<ApplicationEmailModel>()), Times.Once);
        }
        
        [TestMethod]
        public async Task AccountsControllerWithOneDelegatedUsers_AndApplicationIsApproved_ShouldReturn200_WhenSuccess()
        {
            // Arrange
            var govNotificationRequestModel = new GovNotificationRequestModel
            {
                Decision = "Approved",
                OrganisationName = "org 13",
                OrganisationNumber = "123456789",
                ApprovedUser =
                    new UserRequestModel {Email = "abc@hotmail.com", UserFirstName = "first", UserSurname = "last"},
                DelegatedUsers = new List<UserRequestModel>
                {new()
                {
                    Email = "test@test",
                    UserFirstName = "Test",
                    UserSurname = "Test"
                }},
                RejectionComments = "not good enough",
                RegulatorRole = "Approved",
            };

            _messagingServiceMock.Setup(x => x.DelegatedPersonAccepted(It.IsAny<ApplicationEmailModel>())).Returns(new List<string>{"Test"});

            // Act
            var govNotification = await _sut.GovNotification(govNotificationRequestModel);
            
            // Assert
            govNotification.Should().NotBeNull();
            var result = govNotification as ObjectResult;
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(200);
            _messagingServiceMock.Verify(x => x.DelegatedPersonAccepted(It.IsAny<ApplicationEmailModel>()), Times.Once);
        }
        
        [TestMethod]
        public async Task AccountsControllerWithRegulatorRoleApproved_AndApplicationIsRejected_ShouldReturn200_WhenSuccess()
        {
            // Arrange
            var govNotificationRequestModel = new GovNotificationRequestModel
            {
                Decision = "Rejected",
                OrganisationName = "org 13",
                OrganisationNumber = "123456789",
                ApprovedUser =
                    new UserRequestModel {Email = "abc@hotmail.com", UserFirstName = "first", UserSurname = "last"},
                DelegatedUsers = new List<UserRequestModel>
                {new()
                {
                    Email = "test@test",
                    UserFirstName = "Test",
                    UserSurname = "Test"
                }},
                RejectionComments = "not good enough",
                RegulatorRole = "Packaging.ApprovedPerson"
            };

            _messagingServiceMock.Setup(x => x.ApprovedPersonRejected(It.IsAny<ApplicationEmailModel>())).Returns(new List<string>{"Test"});

            // Act
            var govNotification = await _sut.GovNotification(govNotificationRequestModel);

            // Assert
            govNotification.Should().NotBeNull();
            var result = govNotification as ObjectResult;
            result?.StatusCode.Should().Be(200);
            _messagingServiceMock.Verify(x => x.ApprovedPersonRejected(It.IsAny<ApplicationEmailModel>()), Times.Once);
        }
        
        [TestMethod]
        public async Task AccountsControllerWithRegulatorRoleDelegated_AndApplicationIsRejected_ShouldReturn200_WhenSuccess()
        {
            // Arrange
            var govNotificationRequestModel = new GovNotificationRequestModel
            {
                Decision = "Rejected",
                OrganisationName = "org 13",
                OrganisationNumber = "123456789",
                ApprovedUser =
                    new UserRequestModel {Email = "abc@hotmail.com", UserFirstName = "first", UserSurname = "last"},
                DelegatedUsers = new List<UserRequestModel>
                {new()
                {
                    Email = "test@test",
                    UserFirstName = "Test",
                    UserSurname = "Test"
                }},
                RejectionComments = "not good enough",
                RegulatorRole = "Packaging.DelegatedPerson"
            };

            _messagingServiceMock.Setup(x => x.DelegatedPersonRejected(It.IsAny<ApplicationEmailModel>())).Returns(new List<string>{"Test"});

            // Act
            var govNotification = await _sut.GovNotification(govNotificationRequestModel);

            // Assert
            govNotification.Should().NotBeNull();
            var result = govNotification as ObjectResult;
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(200);
            _messagingServiceMock.Verify(x => x.DelegatedPersonRejected(It.IsAny<ApplicationEmailModel>()), Times.Once);
        }

        [TestMethod]
        public async Task AccountsController_Throws_BadRequest_On_Error()
        {
            // Arrange
            var govNotificationRequestModel = GetGovNotificationRequestModel();
            govNotificationRequestModel.Decision = "";

            // Act
            var result = _sut.GovNotification(govNotificationRequestModel);

            result.Should().NotBeNull();
        }

        [TestMethod]
        public async Task ApplicationController_Throws_500_When_MessagingService_Throws_Error()
        {
            // Arrange
            var govNotificationRequestModel = new GovNotificationRequestModel
            {
                Decision = "Approved",
                OrganisationName = "org 13",
                OrganisationNumber = "123456789",
                ApprovedUser = new UserRequestModel()
                    {Email = "abc@hotmail.com", UserFirstName = "first", UserSurname = "last"},
                DelegatedUsers = new(),
                RegulatorRole = "Approved",
            };

            _messagingServiceMock.Setup(x => x.ApprovedPersonAccepted(It.IsAny<ApplicationEmailModel>()))
                .Throws(new Exception("Invalid Request"));

            // Act
            await Assert.ThrowsExactlyAsync<Exception>(async () => await _sut.GovNotification(govNotificationRequestModel));
        }

        private static GovNotificationRequestModel GetGovNotificationRequestModel()
        {
            return new GovNotificationRequestModel
            {
                Decision = "Approved",
                OrganisationName = "org 13",
                OrganisationNumber = "123456789",
                ApprovedUser =
                    new UserRequestModel() {Email = "abc@hotmail.com", UserFirstName = "first", UserSurname = "last"},
                DelegatedUsers = new(),
                RejectionComments = "not good enough",
                RegulatorRole = "Approved",
            };
        }

        private static ApplicationEmailModel GetApplicationEmailModel()
        {
            var applicationEmailModel = new ApplicationEmailModel()
            {
                ApprovedPerson = new UserEmailModel()
                    {Email = "abc@hotmail.com", FirstName = "first", LastName = "last"},
                OrganisationNumber = "123456789",
                OrganisationName = "org 13",
                AccountLoginUrl = "http://www.google.com",
                RejectionComments = "not good enough",
            };

            return applicationEmailModel;
        }
    }
}