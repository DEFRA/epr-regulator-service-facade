using System.Net;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.Core.Models.Organisations;
using EPR.RegulatorService.Facade.Core.Services.Producer;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;

namespace EPR.RegulatorService.Facade.Tests.API.Controllers.OrganisationsSearch
{
    [TestClass]
    public class OrganisationsSearchControllerTests : Controller
    {
        private readonly NullLogger<OrganisationsSearchController> _nullLogger = new();
        private readonly Mock<IProducerService> _mockProducerService = new();
        private readonly Mock<IRegulatorOrganisationService> _mockRegulatorOrganisationService = new();
        private OrganisationsSearchController _sut;

        private readonly Guid _organisationExternalId = Guid.NewGuid();
        private readonly int _currentPage = 1;
        private const int PageSize = 10;
        private const string OrganisationName = "Org";

        [TestInitialize]
        public void Setup()
        {
            _sut = new OrganisationsSearchController(_nullLogger, _mockProducerService.Object, _mockRegulatorOrganisationService.Object);
            _sut.AddDefaultContextWithOid(_organisationExternalId, "TestAuth");
        }

        [TestMethod]
        public async Task When_GetOrganisationsBySearchTerm_Is_Called_And_Request_Is_Valid_Then_Return_200_Success()
        {
            // Arrange
            var organisations = new OrganisationSearchResult();
            _mockProducerService.Setup(x =>
                x.GetOrganisationsBySearchTerm(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(organisations))
            });

            // Act
            var result = await _sut.GetOrganisationsBySearchTerm(_currentPage, PageSize, OrganisationName);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task When_GetOrganisationsBySearchTerm_Is_Called_And_Request_Is_Invalid_Then_Return_400_Bad_Request()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationsBySearchTerm(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.GetOrganisationsBySearchTerm(_currentPage, PageSize, OrganisationName);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }
        
        [TestMethod]
        public async Task When_GetOrganisationsBySearchTerm_Is_Called_And_Request_Is_Not_Successful_Then_Return_InternalServerError()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationsBySearchTerm(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Conflict
            });

            // Act
            var result = await _sut.GetOrganisationsBySearchTerm(_currentPage, PageSize, OrganisationName);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
        
        [TestMethod]
        public async Task On_Getting_Organisation_Details_Valid_Result_Can_Be_Parsed()
        {
            // Arrange
            var companyObject = new OrganisationDetailResults();
            _mockProducerService.Setup(x =>
                x.GetOrganisationDetails(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(companyObject))
            });

            // Act
            var result = await _sut.OrganisationDetails(Guid.NewGuid());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task On_Getting_Organisation_Details_Http_Error_Result_Is_Handled()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationDetails(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ThrowsAsync(new HttpRequestException("Test exception", null, HttpStatusCode.BadRequest));

            // Act
            var result = await _sut.OrganisationDetails(Guid.NewGuid());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }
        
        [TestMethod]
        public async Task On_Getting_Organisation_Details_Http_Unsuccessful_Result_Is_Handled()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.GetOrganisationDetails(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

            // Act
            var result = await _sut.OrganisationDetails(Guid.NewGuid());

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
        
        [TestMethod]
        public async Task When_GetProducerOrganisationsUsersByExternalId_Is_Called_And_Request_Is_Valid_Then_Return_200_Success()
        {
            // Arrange
            var users = new List<OrganisationUserOverviewResponseModel>();
            _mockRegulatorOrganisationService.Setup(x =>
                x.GetUsersByOrganisationExternalId(It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(users))
            });

            // Act
            var result = await _sut.GetUsersByOrganisationExternalId(_organisationExternalId);

            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }

        [TestMethod]
        public async Task When_RemoveApprovedPerson_Valid_Result_Is_Successful()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK
            });
            
            
            // Act
            var result = await _sut.RemoveApprovedPerson(Guid.NewGuid(),Guid.NewGuid());
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task When_RemoveApprovedPerson_Http_Error_Result_Is_Handled()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest
            });
            
            
            // Act
            var result = await _sut.RemoveApprovedPerson(Guid.NewGuid(),Guid.NewGuid());
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be(400);
        }

        [TestMethod]
        public async Task When_RemoveApprovedPerson_Http_Unsuccessful_Result_Is_Handled()
        {
            // Arrange
            _mockProducerService.Setup(x =>
                x.RemoveApprovedUser(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.InternalServerError
            });
            
            
            // Act
            var result = await _sut.RemoveApprovedPerson(Guid.NewGuid(),Guid.NewGuid());
            
            // Assert
            result.Should().NotBeNull();
            var statusCodeResult = result as StatusCodeResult;
            statusCodeResult?.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}