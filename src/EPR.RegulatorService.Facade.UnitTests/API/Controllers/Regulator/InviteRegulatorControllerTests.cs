using System.Net;
using System.Text.Json;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Models;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.Core.Services.ServiceRoles;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.Regulator
{
    [TestClass]
    public class InviteRegulatorControllerTests
    {
        private readonly Guid _authorizationId = Guid.NewGuid();

        private InviteRegulatorController _sut;
        private Mock<ILogger<InviteRegulatorController>> _logger = null!;
        private Mock<IServiceRolesLookupService> _mockServiceRolesLookupService = null!;
        private Mock<IRegulatorOrganisationService> _mockRegulatorOrganisationService = null!;
        private const string Token = "SomeToken";

        [TestInitialize]
        public void Setup()
        {
            _mockServiceRolesLookupService = new Mock<IServiceRolesLookupService>();
            _mockRegulatorOrganisationService = new Mock<IRegulatorOrganisationService>();

            _mockServiceRolesLookupService.Setup(x => x.GetServiceRoles()).Returns(new List<ServiceRolesLookupModel>
            {
                new ServiceRolesLookupModel
                {
                    Key = "Regulator.Admin",
                    PersonRoleId = 1,
                    ServiceRoleId = 4
                },
                new ServiceRolesLookupModel
                {
                    Key = "Regulator.Basic",
                    PersonRoleId = 2,
                    ServiceRoleId = 5
                }
            });

            _logger = new Mock<ILogger<InviteRegulatorController>>();

            _sut = new InviteRegulatorController(_logger.Object, _mockRegulatorOrganisationService.Object, _mockServiceRolesLookupService.Object);
            _sut.AddDefaultContextWithOid(_authorizationId, "TestAuth");
        }

        [TestMethod]
        public async Task CreateInviteEnrollment_SendValidRequest_ReturnOkObjectResult()
        {
            // Arrange
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvites(It.IsAny<AddInviteUserRequest>())).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode =  HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new AddRemoveApprovedPersonResponseModel { InviteToken = Token }))
                });

            // Act
            var result = await _sut.CreateInviteEnrollment(request) as OkObjectResult;

            // Assert
            result!.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task CreateInviteEnrollment_SendInValidRequest_ReturnBadRequest()
        {
            // Arrange
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvites(It.IsAny<AddInviteUserRequest>())).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            // Act
            var result = await _sut.CreateInviteEnrollment(request) as BadRequestResult;

            // Assert
            result!.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task EnrolInvitedUser_SendValidRequest_ReturnNoContent()
        {
            // Arrange
            var request = GetEnrolInvitedUserRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorEnrollment(request)).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                });

            // Act
            var result = await _sut.EnrolInvitedUser(request) as NoContentResult;

            // Assert
            result!.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task EnrolInvitedUser_SendInValidRequest_ReturnBadRequestResult()
        {
            // Arrange
            var request = GetEnrolInvitedUserRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorEnrollment(request)).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                });

            // Act
            var result = await _sut.EnrolInvitedUser(request) as BadRequestResult;

            // Assert
            result!.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }
        
        [TestMethod]
        public async Task GetInvitedEnrollment_WithNoInvite_Returns400Code()
        {
            // Arrange
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvited(It.IsAny<Guid>(),It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            // Act
            var result = await _sut.GetInvitedEnrollment(request.Email);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult.StatusCode.Should().Be(400);
        }
        
        [TestMethod]
        public async Task GetInvitedEnrollment_WithInvite_Return200Code()
        {
            // Arrange
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvited(It.IsAny<Guid>(),It.IsAny<string>())).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

            // Act
            var result = await _sut.GetInvitedEnrollment(request.Email);

            // Assert
            result.Should().NotBeNull();
            var okObjectResult = result as OkObjectResult; 
            okObjectResult.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task GetInvitedEnrollment_WithErrorInRegulatorInvited_Returns500Code()
        {
            // Arrange
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvited(It.IsAny<Guid>(), It.IsAny<string>())).ThrowsAsync(new NotSupportedException());

            // Act
            var result = await _sut.GetInvitedEnrollment(request.Email);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult.StatusCode.Should().Be(500);
        }
        
        [TestMethod]
        public async Task GetInvitedEnrollment_WithEmptyUserId_Returns500Code()
        {
            // Arrange
            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvited(It.IsAny<Guid>(), It.IsAny<string>())).ThrowsAsync(new NotSupportedException());

            // Act
            var result = await _sut.GetInvitedEnrollment(request.Email);

            // Assert
            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
        }
        
        [TestMethod]
        public async Task GetInvitedEnrollment_WithInvalidModel_ReturnsResponseWithValidationProblemDetails()
        {
            // Arrange
            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");
            _sut.ModelState.AddModelError("Test","Test err");
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvited(It.IsAny<Guid>(), It.IsAny<string>())).ThrowsAsync(new NotSupportedException());

            // Act
            var result = await _sut.GetInvitedEnrollment(request.Email);

            // Assert
            result.Should().NotBeNull();
            var objectResult = result as ObjectResult;
            objectResult.Value.Should().BeOfType(typeof(ValidationProblemDetails));
        }

        [TestMethod]
        public async Task Invalid_CreateInviteEnrollment_ReturnBadResultResult()
        {
            // Arrange
            var request = GetInviteEnrollmentRequest();

            // Act
            var result = await _sut.CreateInviteEnrollment(request) as OkObjectResult;

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task EnrolInvitedUser_SendInValidRequest_ReturnException()
        {
            // Arrange
            var request = GetEnrolInvitedUserRequest();

            // Act
            var result = await _sut.EnrolInvitedUser(request) as BadRequestResult;

            // Assert
            result!.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateInviteEnrollment_InValidUser_ReturnNullObjectResult()
        {
            // Arrange
            var request = GetInviteEnrollmentRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorInvites(It.IsAny<AddInviteUserRequest>())).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new AddRemoveApprovedPersonResponseModel { InviteToken = Token }))
                });
            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.CreateInviteEnrollment(request) as OkObjectResult;

            // Assert
            result!.Should().BeNull();
        }

        [TestMethod]
        public async Task EnrolInvitedUser_SendInValidUser_ReturnNull()
        {
            // Arrange
            var request = GetEnrolInvitedUserRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.RegulatorEnrollment(request)).ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                });

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            // Act
            var result = await _sut.EnrolInvitedUser(request) as NoContentResult;

            // Assert
            result!.Should().BeNull();
        }

        private static EnrolInvitedUserRequest GetEnrolInvitedUserRequest()
        {
            return new EnrolInvitedUserRequest
            {
                Email = "system@test",
                FirstName = "Test",
                LastName = "Test",
                InviteToken = Token,
                UserId = Guid.NewGuid().ToString(),
            };
        }

        private static RegulatorInviteEnrollmentRequest GetInviteEnrollmentRequest()
        {
            return new RegulatorInviteEnrollmentRequest
            {
                UserId = Guid.NewGuid(),
                Email = "system@test",
                OrganisationId = Guid.NewGuid(),
                PersonRoleId = 1,
                RoleKey = "Regulator.Admin",
                ServiceRoleId = 4,
            };
        }
    }
}
