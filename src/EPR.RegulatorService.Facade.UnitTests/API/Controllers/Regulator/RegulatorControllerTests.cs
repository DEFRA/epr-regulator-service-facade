using System.Net;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Services.Application;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.Regulator
{
    [TestClass]
    public class ApplicationControllerTests : Controller
    {
        private readonly NullLogger<RegulatorController> _nullLogger = new();
        private readonly Mock<IApplicationService> _mockRegulatorService = new();
        private readonly Mock<IMessagingService> _messagingServiceMock = new();
        private RegulatorController _sut;

        private readonly Guid _oid = Guid.NewGuid();
        private readonly int _currentPage = 1;
        private const int PageSize = 10;
        private const string OrganisationName = "Org";
        private const string ServiceRoleId = "ServiceRoleId";
        private readonly Guid _organisationId = new Guid("28b527a8-af1f-42ff-b120-f4e94e40b637");

        [TestInitialize]
        public void Setup()
        {
            _sut = new RegulatorController(_nullLogger, _mockRegulatorService.Object, _messagingServiceMock.Object);
            _sut.AddDefaultContextWithOid(_oid, "TestAuth");
        }

        [TestMethod]
        public async Task Should_return_BadRequest_when_GetPendingRegulators_throws_400()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.PendingApplications(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.PendingApplications(_currentPage, PageSize, OrganisationName, ServiceRoleId);

            // Assert
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }
        
        [TestMethod]
        public async Task Should_return_ServerError_When_UserId_Is_Empty()
        {
            // Arrange
            var enrolments = new List<OrganisationEnrolments>();
            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");
            _mockRegulatorService.Setup(x =>
                x.PendingApplications(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>())
            ).ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(enrolments))
            });

            // Act
            var result = await _sut.PendingApplications(_currentPage, PageSize, OrganisationName, ServiceRoleId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }
        
        [TestMethod]
        public async Task Should_return_InternalServerError_when_GetPendingRegulators_throws_500()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.PendingApplications(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.InternalServerError));

            // Act
            var result = await _sut.PendingApplications(_currentPage, PageSize, OrganisationName, ServiceRoleId);

            // Assert
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public async Task Should_return_OkResult_200_when_valid_request()
        {
            // Arrange
            var enrolments = new List<OrganisationEnrolments> { };

            _mockRegulatorService.Setup(x =>
                    x.PendingApplications(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                        It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(enrolments))
                });

            // Act
            var result = await _sut.PendingApplications(_currentPage, PageSize, OrganisationName, ServiceRoleId);

            // Assert
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task
            When_Organisations_Pending_Application_Is_Called_And_Request_Is_Invalid_Then_Return_400_Bad_Request()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.GetOrganisationPendingApplications(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.GetOrganisationApplications(_organisationId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task
            When_Organisations_Pending_Application_Is_Called_And_User_Is_Not_Authorised_Then_Return_403_Forbidden()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.GetOrganisationPendingApplications(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.Forbidden));

            // Act
            var result = await _sut.GetOrganisationApplications(_organisationId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(403);
        }

        [TestMethod]
        public async Task
            When_Organisations_Pending_Application_Is_Called_And_Request_Is_Valid_Then_Return_200_Success()
        {
            // Arrange
            var enrolments = new ApplicationEnrolmentDetailsResponse();
            _mockRegulatorService.Setup(x =>
                x.GetOrganisationPendingApplications(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(enrolments))
            });

            // Act
            var result = await _sut.GetOrganisationApplications(_organisationId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task When_Update_Enrolment_Is_Called_And_Request_Is_Invalid_Then_Return_400_Bad_Request()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.UpdateEnrolment(It.IsAny<ManageRegulatorEnrolmentRequest>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.UpdateEnrolment(
                new UpdateEnrolmentRequest
                {
                    EnrolmentId = _organisationId,
                    EnrolmentStatus = "Approved",
                    Comments = String.Empty
                });

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task When_Update_Enrolment_Is_Called_And_User_Is_Not_Authorised_Then_Return_403_Forbidden()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.UpdateEnrolment(It.IsAny<ManageRegulatorEnrolmentRequest>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.Forbidden));

            // Act
            var result = await _sut.UpdateEnrolment(
                new UpdateEnrolmentRequest
                {
                    EnrolmentId = _organisationId,
                    EnrolmentStatus = "Approved",
                    Comments = String.Empty
                });

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(403);
        }

        [TestMethod]
        public async Task When_Update_Enrolment_Is_Called_And_Request_Is_Valid_Then_Return_204_Success()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.UpdateEnrolment(It.IsAny<ManageRegulatorEnrolmentRequest>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NoContent
            });

            // Act
            var result = await _sut.UpdateEnrolment(
                new UpdateEnrolmentRequest
                {
                    EnrolmentId = _organisationId,
                    EnrolmentStatus = "Approved",
                    Comments = String.Empty
                });

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(204);
        }

        [TestMethod]
        public async Task
            When_Transfer_Organisation_Nation_Is_Called_And_Request_Is_Invalid_Then_Return_400_Bad_Request()
        {
            // Arrange
            var request = new OrganisationTransferNationRequest
            {
                OrganisationId = _organisationId,
                TransferNationId = 3,
                TransferComments = String.Empty
            };
            _mockRegulatorService.Setup(x =>
                x.TransferOrganisationNation(It.IsAny<OrganisationTransferNationRequest>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.TransferOrganisationNation(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task
            When_Transfer_Organisation_Nation_Is_Called_And_User_Is_Not_Authorised_Then_Return_403_Forbidden()
        {
            // Arrange
            var request = new OrganisationTransferNationRequest
            {
                OrganisationId = _organisationId,
                TransferNationId = 3,
                TransferComments = "Incorrect nation"
            };
            _mockRegulatorService.Setup(x =>
                x.TransferOrganisationNation(It.IsAny<OrganisationTransferNationRequest>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.Forbidden));

            // Act
            var result = await _sut.TransferOrganisationNation(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(403);
        }

        [TestMethod]
        public async Task When_Transfer_Organisation_Nation_Is_Called_And_Request_Is_Valid_Then_Return_204_Success()
        {
            // Arrange
            var request = new OrganisationTransferNationRequest
            {
                OrganisationId = _organisationId,
                TransferNationId = 3,
                TransferComments = String.Empty
            };
            _mockRegulatorService.Setup(x =>
                x.TransferOrganisationNation(It.IsAny<OrganisationTransferNationRequest>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.NoContent
            });

            // Act
            var result = await _sut.TransferOrganisationNation(request);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(204);
        }

        [TestMethod]
        public async Task When_User_Organisations_Is_Called_And_Request_Is_Invalid_Then_Return_400_Bad_Request()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.GetUserOrganisations(It.IsAny<Guid>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.GetUserDetails();

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task When_User_Organisations_Is_Called_And_User_Is_Not_Authorised_Then_Return_403_Forbidden()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.GetUserOrganisations(It.IsAny<Guid>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.Forbidden));

            // Act
            var result = await _sut.GetUserDetails();

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(403);
        }

        [TestMethod]
        public async Task When_User_Organisations_Is_Called_And_Request_Is_Valid_Then_Return_200_Success()
        {
            // Arrange
            var enrolments = new ApplicationEnrolmentDetailsResponse();
            _mockRegulatorService.Setup(x =>
                x.GetUserOrganisations(It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(enrolments))
            });

            // Act
            var result = await _sut.GetUserDetails();

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task When_UserInviteExist_ReturnValidToken()
        {
            // Arrange
            var enrolments = new ApplicationEnrolmentDetailsResponse();

            _mockRegulatorService.Setup(x =>
                x.GetUserOrganisations(It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(enrolments))
            });

            // Act
            var result = await _sut.GetUserDetails();

            // Assert
            result.Should().NotBeNull();
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int) HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task When_UserInviteDoesNotExist_ReturnEmptyToken()
        {
            // Arrange
            _mockRegulatorService.Setup(x =>
                x.GetUserOrganisations(It.IsAny<Guid>())
            ).Throws(new Exception("Some exception"));

            // Act
            var result = await _sut.GetUserDetails();

            // Assert
            result.Should().NotBeNull();
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int) HttpStatusCode.InternalServerError);
        }
    }
}