using System.ComponentModel.DataAnnotations;
using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.API.Handlers;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.OrganisationRegistration;

[TestClass]
public class OrganisationRegistrationControllerTests : Controller
{
    private readonly NullLogger<OrganisationRegistrationController> _nullLogger = new();
    private readonly Mock<ICommonDataService> _mockCommonDataService = new();
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private OrganisationRegistrationController _sut;
    private readonly Guid _oid = Guid.NewGuid();
    private readonly string testDataFilePath = "API/Controllers/OrganisationRegistration/paginateddummydata.json";

    public TestContext TestContext
    {
        get; set;
    }

    [TestInitialize]
    public void Setup()
    {
        _sut = new OrganisationRegistrationController(
            _nullLogger,
            _mockCommonDataService.Object);

        _sut.AddDefaultContextWithOid(_oid, "TestAuth");
    }

    private void setupNoDataForCommonData()
    {
        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _mockCommonDataService.Setup(m=>m.GetOrganisationRegistrations<JsonOrganisationRegistrationHandler>(It.IsAny<GetOrganisationRegistrationRequest>()))
            .ReturnsAsync(handlerResponse);
    }

    private void setupAllDataForCommonDataAsync()
    {
         var testJson = TestRunDataHelper.LoadDataFile(testDataFilePath, TestContext);

        var handlerResponse =
        _fixture
            .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.OK)
                    .With(x => x.Content, new StringContent(testJson))
                    .Create();

        _mockCommonDataService.Setup(m => m.GetOrganisationRegistrations<JsonOrganisationRegistrationHandler>(It.IsAny<GetOrganisationRegistrationRequest>()))
            .ReturnsAsync(handlerResponse);
    }

    [TestMethod]
    public async Task When_calling_getorganisationregistrations_without_filter_parameter_returns_400_bad_response()
    {
        //Arrange
        OrganisationRegistrationFilter request = new();
        ValidateModel(_sut, request);

        //Act
        var response = await _sut.GetOrganisationRegistrations(request);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>();
        var badRequestObjectResult = response as BadRequestObjectResult;
        badRequestObjectResult!.StatusCode.Should().Be(400);

        badRequestObjectResult.Value.Should().BeOfType<ValidationProblemDetails>();
    }

    [TestMethod]
    public async Task When_calling_getorganisationregistrations_that_throws_any_internal_exception_returns_400_bad_response()
    {
        //Arrange
        OrganisationRegistrationFilter request = new() { PageNumber = 1, PageSize = 2};
        ValidateModel(_sut, request);
        setupNoDataForCommonData();
        //Act
        var response = await _sut.GetOrganisationRegistrations(request);

        // Assert
        response.Should().BeOfType<StatusCodeResult>();
        var statusCodeResult = response as StatusCodeResult;
        statusCodeResult.StatusCode.Should().Be(500);
        
    }

    [TestMethod]
    public async Task When_calling_getorganisationregistrations_that_receives_data_returns_200_ok_response()
    {
        //Arrange
        OrganisationRegistrationFilter request = new() { PageNumber = 1, PageSize = 2 };
        ValidateModel(_sut, request);
        setupAllDataForCommonDataAsync();
        
        //Act
        var response = await _sut.GetOrganisationRegistrations(request);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okObjectResult = response as OkObjectResult;
        okObjectResult.StatusCode.Should().Be(200);
        okObjectResult.Value.Should().BeOfType<PaginatedResponse<OrganisationRegistrationSummaryResponse>>();
    }

    [TestMethod]
    public async Task When_calling_getorganisationregistrations_and_exception_occurs_then_we_expect_an_internalservererror()
    {
        //Arrange
        OrganisationRegistrationFilter request = new();
        ValidateModel(_sut, request);

        Mock<IOrganisationRegistrationHandlers> mockHandler = new();

        mockHandler.Setup(x => x.ManageModelState(It.IsAny<ModelStateDictionary>())).Throws(new InvalidDataException());

        _sut.RegistrationHandler = mockHandler.Object;
        
        //Act
        var response = await _sut.GetOrganisationRegistrations(request);

        // Assert
        response.Should().BeOfType<StatusCodeResult>();
        ((StatusCodeResult)response).StatusCode.Should().Be(500);
    }

    private void ValidateModel(ControllerBase controller, object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
            }
        }
    }
}
