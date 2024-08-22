using System.Net;
using System.Text.Json;
using System.Text;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Models.Accounts;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Services.Application;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using EPR.RegulatorService.Facade.Core.Models.Accounts.EmailModels;

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
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.OK);
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
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task PendingUserChangeRequests_Pass_InvalidRequest_Return_InternalServerError()
        {
            // Arrange
            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.PendingUserChangeRequests(1, 1, "", "") as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task PendingUserChangeRequests_Pass_ValidRequest_Return_OK()
        {
            // Arrange
            _mockRegulatorService.Setup(x => x.GetPendingUserDetailChangeRequestsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(It.IsAny<OrganisationEnrolments>()))
                });

            // Act
            var result = await _sut.PendingUserChangeRequests(It.IsAny<int>(), It.IsAny<int>(), "Org", "") as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task PendingUserChangeRequests_No_Content_ToSerialise_Return_500()
        {
            // Arrange
            _mockRegulatorService.Setup(x => x.GetPendingUserDetailChangeRequestsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new HttpResponseMessage());

            // Act
            var result = await _sut.PendingUserChangeRequests(It.IsAny<int>(), It.IsAny<int>(), "Org", "") as StatusCodeResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task GetUserDetailChangeRequest_Pass_InvalidRequest_Return_InternalServerError()
        {
            // Arrange
            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.GetUserDetailChangeRequest(It.IsAny<Guid>()) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task GetUserDetailChangeRequest_No_Content_ToSerialise_Return_500()
        {
            // Arrange
            _mockRegulatorService.Setup(x => x.GetOrganisationPendingApplications(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new HttpResponseMessage());

            // Act
            var result = await _sut.GetUserDetailChangeRequest(It.IsAny<Guid>()) as StatusCodeResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task GetUserDetailChangeRequest_Pass_ValidRequest_Return_OK()
        {
            // Arrange
            _mockRegulatorService.Setup(x => x.GetOrganisationPendingApplications(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new HttpResponseMessage()
                { Content = new StringContent(JsonConvert.SerializeObject(It.IsAny<ApplicationEnrolmentDetailsResponse>())) });

            // Act
            var result = await _sut.GetUserDetailChangeRequest(It.IsAny<Guid>()) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(null);

            // Assert
            var badRequestResult = result as ObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            _mockRegulatorService.Verify(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(null),
                Times.Never());
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_ReturnsBadRequest_ForAnInvalidRequest()
        {
            // Arrange
            var request = new UpdateUserDetailsRequest
            {
                ChangeHistoryExternalId = default,
                HasRegulatorAccepted = false,
                RegulatorComment = null
            };

            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(request);

            // Assert
            var badRequestResult = result as ObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            _mockRegulatorService.Verify(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()),
                Times.Never());
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_ReturnsOk_ForAValidRequest()
        {
            // Arrange
            var request = new UpdateUserDetailsRequest
            {
                ChangeHistoryExternalId = Guid.NewGuid(),
                HasRegulatorAccepted = true,
                RegulatorComment = "Approved"
            };

            var responseContent = new RegulatorUserDetailsUpdateResponse
            {
                HasUserDetailsChangeAccepted = true,
                ChangeHistory = new ChangeHistoryModel
                {
                    Nation = "Nation",
                    EmailAddress = "user@example.com",
                    NewValues = new UserDetailsChangeModel
                    {
                        FirstName = "NewFirstName",
                        LastName = "NewLastName",
                        JobTitle = "NewJobTitle"
                    },
                    OldValues = new UserDetailsChangeModel
                    {
                        FirstName = "OldFirstName",
                        LastName = "OldLastName",
                        JobTitle = "OldJobTitle"
                    },
                    ApproverComments = "Approved",
                    OrganisationId = 1,
                    OrganisationName = "OrganisationName",
                    OrganisationType = "OrganisationType",
                    OrganisationReferenceNumber = "OrgRef123",
                    ServiceRole = "ServiceRole",
                    BusinessAddress = new AddressModel
                    {
                        Street = "123 Street",
                        Country = "Country",
                        Postcode = "Postcode"
                    }
                }
            };

            var responseContentJson = System.Text.Json.JsonSerializer.Serialize(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContentJson, Encoding.UTF8, "application/json")
            };

            _mockRegulatorService.Setup(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(RegulatorUserDetailsUpdateResponse));

            _mockRegulatorService.Verify(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()),
                Times.Once());

            _messagingServiceMock.Verify(service =>
                service.SendAcceptedUserDetailChangeEmail(It.IsAny<UserDetailsChangeNotificationEmailInput>()),
                Times.Once());

            _messagingServiceMock.Verify(service =>
                service.SendRejectedUserDetailChangeEmail(It.IsAny<UserDetailsChangeNotificationEmailInput>()),
                Times.Never());
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_ReturnsOk_ForARejectedRequest()
        {
            // Arrange
            var request = new UpdateUserDetailsRequest
            {
                ChangeHistoryExternalId = Guid.NewGuid(),
                HasRegulatorAccepted = false,
                RegulatorComment = "Rejected"
            };

            var responseContent = new RegulatorUserDetailsUpdateResponse
            {
                HasUserDetailsChangeRejected = true,
                ChangeHistory = new ChangeHistoryModel
                {
                    Nation = "Nation",
                    EmailAddress = "user@example.com",
                    NewValues = new UserDetailsChangeModel
                    {
                        FirstName = "NewFirstName",
                        LastName = "NewLastName",
                        JobTitle = "NewJobTitle"
                    },
                    OldValues = new UserDetailsChangeModel
                    {
                        FirstName = "OldFirstName",
                        LastName = "OldLastName",
                        JobTitle = "OldJobTitle"
                    },
                    ApproverComments = "Rejected",
                    OrganisationId = 1,
                    OrganisationName = "OrganisationName",
                    OrganisationType = "OrganisationType",
                    OrganisationReferenceNumber = "OrgRef123",
                    ServiceRole = "ServiceRole",
                    BusinessAddress = new AddressModel
                    {
                        Street = "123 Street",
                        Country = "Country",
                        Postcode = "Postcode"
                    }
                }
            };

            var responseContentJson = System.Text.Json.JsonSerializer.Serialize(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContentJson, Encoding.UTF8, "application/json")
            };

            _mockRegulatorService.Setup(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsInstanceOfType(okResult.Value, typeof(RegulatorUserDetailsUpdateResponse));

            _mockRegulatorService.Verify(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()),
                Times.Once());

            _messagingServiceMock.Verify(service =>
                service.SendRejectedUserDetailChangeEmail(It.IsAny<UserDetailsChangeNotificationEmailInput>()),
                Times.Once());

            _messagingServiceMock.Verify(service =>
                service.SendAcceptedUserDetailChangeEmail(It.IsAny<UserDetailsChangeNotificationEmailInput>()),
                Times.Never());
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_FailedRequest_ReturnsErrorResponse()
        {
            // Arrange
            var request = new UpdateUserDetailsRequest
            {
                ChangeHistoryExternalId = Guid.NewGuid(),
                HasRegulatorAccepted = true,
                RegulatorComment = "Approved"
            };

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);

            _mockRegulatorService.Setup(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()))
                .ReturnsAsync(response);

            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(request);

            // Assert
            var badRequestResult = result as StatusCodeResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);

            _mockRegulatorService.Verify(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var request = new UpdateUserDetailsRequest
            {
                ChangeHistoryExternalId = Guid.NewGuid(),
                HasRegulatorAccepted = true,
                RegulatorComment = "Approved"
            };

            var errorMessage = "Some unexpected error";

            _mockRegulatorService.Setup(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(request);

            // Assert
            var internalServerErrorResult = result as StatusCodeResult;
            Assert.IsNotNull(internalServerErrorResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, internalServerErrorResult.StatusCode);

            _mockRegulatorService.Verify(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()),
                Times.Once());
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_Pass_InvalidRequest_Return_InternalServerError()
        {
            // Arrange
            var request = new Facade.Core.Models.Requests.UpdateUserDetailsRequest { ChangeHistoryExternalId = Guid.NewGuid(), HasRegulatorAccepted = true };

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(request) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [TestMethod]
        public async Task AcceptOrRejectUserDetailsChangeRequest_ThrowsException_WhenSendingEmailFails()
        {
            // Arrange
            var request = new UpdateUserDetailsRequest
            {
                ChangeHistoryExternalId = Guid.NewGuid(),
                HasRegulatorAccepted = false,
                RegulatorComment = "Rejected"
            };

            var responseContent = new RegulatorUserDetailsUpdateResponse
            {
                HasUserDetailsChangeRejected = true,
                ChangeHistory = new ChangeHistoryModel
                {
                    EmailAddress = "user@example.com",
                    ApproverComments = "Rejected",
                    OrganisationId = 1,
                    OrganisationName = "OrganisationName",
                    OrganisationType = "OrganisationType",
                    OrganisationReferenceNumber = "OrgRef123",
                    ServiceRole = "ServiceRole",
                    BusinessAddress = new AddressModel
                    {
                        Street = "123 Street",
                        Country = "Country",
                        Postcode = "Postcode"
                    }
                }
            };

            var responseContentJson = System.Text.Json.JsonSerializer.Serialize(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseContentJson, Encoding.UTF8, "application/json")
            };

            _mockRegulatorService.Setup(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()))
                .ReturnsAsync(response);

            var errorMessage = $"failed to send Accept Reject User Detail Change Email To for  for externalId {request.ChangeHistoryExternalId}";

            _messagingServiceMock.Setup(service =>
                service.SendAcceptedUserDetailChangeEmail(It.IsAny<UserDetailsChangeNotificationEmailInput>()))
                .Throws(new Exception(errorMessage));

            // Act
            var result = await _sut.AcceptOrRejectUserDetailsChangeRequest(request);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            Assert.IsNotNull(internalServerErrorResult);
            Assert.AreEqual(StatusCodes.Status200OK, internalServerErrorResult.StatusCode);

            _mockRegulatorService.Verify(service =>
                service.AcceptOrRejectUserDetailChangeRequestAsync(It.IsAny<UpdateUserDetailRequest>()),
                Times.Once());

            _messagingServiceMock.Verify(service =>
                service.SendRejectedUserDetailChangeEmail(It.IsAny<UserDetailsChangeNotificationEmailInput>()),
                Times.Never());

            _messagingServiceMock.Verify(service =>
                service.SendAcceptedUserDetailChangeEmail(It.IsAny<UserDetailsChangeNotificationEmailInput>()),
                Times.Never());
        }
    }
}