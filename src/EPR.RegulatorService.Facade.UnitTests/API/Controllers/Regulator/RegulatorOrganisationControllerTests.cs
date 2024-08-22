using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Models.Requests;
using EPR.RegulatorService.Facade.Core.Models.Responses;
using EPR.RegulatorService.Facade.Core.Models.Results;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.Regulator
{
    [TestClass]
    public class RegulatorOrganisationControllerTests

    {
        private readonly Guid _authorizationId = Guid.NewGuid();

        private RegulatorOrganisationController _sut;
        private Mock<ILogger<RegulatorOrganisationController>> _logger = null!;
        private Mock<IRegulatorOrganisationService> _mockRegulatorOrganisationService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockRegulatorOrganisationService = new Mock<IRegulatorOrganisationService>();

            _logger = new Mock<ILogger<RegulatorOrganisationController>>();

            _sut = new RegulatorOrganisationController(_mockRegulatorOrganisationService.Object, _logger.Object);
            _sut.AddDefaultContextWithOid(_authorizationId, "TestAuth");
        }

        [TestMethod]
        public async Task GetRegulatorAccountByNation_SendValidNation_ReturnOkObjectResult()
        {
            // Arrange
            var response = new CheckRegulatorOrganisationExistResponseModel
            {
                CreatedOn = DateTime.Now,
                ExternalId = Guid.NewGuid(),
            };

            _mockRegulatorOrganisationService.Setup(x =>
                x.GetRegulatorOrganisationByNation(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _sut.GetRegulatorAccountByNation(It.IsAny<string>()) as OkObjectResult;

            // Assert
            result!.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task CreateRegulatorOrganisation_SendValidRequest_ReturnCreatedResult()
        {
            // Arrange
            var request = CreateRegulatorAccountRequest();

            var response = new CreateRegulatorOrganisationResponseModel
            {
                CreatedOn = DateTime.Now,
                ExternalId = Guid.NewGuid(),
            };

            _mockRegulatorOrganisationService.Setup(x =>
                x.CreateRegulatorOrganisation(It.IsAny<CreateRegulatorAccountRequest>()))
                .ReturnsAsync(Result<CreateRegulatorOrganisationResponseModel>.SuccessResult(response));

            // Act
            var result = await _sut.CreateRegulatorOrganisation(request) as CreatedAtRouteResult;

            // Assert
            result!.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task CreateRegulatorOrganisation_SendInValidRequest_ReturnBadRequestResult()
        {
            // Arrange
            var request = CreateRegulatorAccountRequest();

            _mockRegulatorOrganisationService.Setup(x =>
                x.CreateRegulatorOrganisation(It.IsAny<CreateRegulatorAccountRequest>()))
                .ReturnsAsync(Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.CreateRegulatorOrganisation(request) as BadRequestResult;

            // Assert
            result!.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task GetRegulatorAccountByNation_Should_return_ServerError_When_UserId_Is_Empty()
        {
            // Arrange
            var response = new CheckRegulatorOrganisationExistResponseModel
            {
                CreatedOn = DateTime.Now,
                ExternalId = Guid.NewGuid(),
            };

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            _mockRegulatorOrganisationService.Setup(x =>
                x.GetRegulatorOrganisationByNation(It.IsAny<string>())).ReturnsAsync(response);

            // Act
            var result = await _sut.GetRegulatorAccountByNation(It.IsAny<string>());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public async Task CreateRegulatorOrganisation_Should_return_ServerError_When_UserId_Is_Empty()
        {
            // Arrange
            var request = CreateRegulatorAccountRequest();

            _sut.AddDefaultContextWithOid(Guid.Empty, "TestAuth");

            _mockRegulatorOrganisationService.Setup(x =>
                x.CreateRegulatorOrganisation(It.IsAny<CreateRegulatorAccountRequest>()))
                .ReturnsAsync(Result<CreateRegulatorOrganisationResponseModel>.FailedResult(string.Empty, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.CreateRegulatorOrganisation(request) as BadRequestResult;

            // Assert
            result!.Should().BeNull();
        }

        private static CreateRegulatorAccountRequest CreateRegulatorAccountRequest()
        {
            return new CreateRegulatorAccountRequest
            {
                Name = "Test",
                NationId = 1,
                ServiceId = "Test Service"
            };
        }
    }
}
