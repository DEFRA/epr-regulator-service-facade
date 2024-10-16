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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.API.Handlers
{
    [TestClass]
    public class OrganisationRegistrationHandlersTests
    {
        private OrganisationRegistrationHandlers _sut;
        private readonly GetOrganisationRegistrationRequest _badParameter = new GetOrganisationRegistrationRequest { };
        private readonly GetOrganisationRegistrationRequest _goodParameter = new GetOrganisationRegistrationRequest { PageNumber = 1, PageSize = 20 };
        private readonly ModelStateDictionary _badModelStateDictionary = new ModelStateDictionary();
        private readonly ModelStateDictionary _goodModelStateDictionary = new ModelStateDictionary();

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Mock<ICommonDataService> _mockCommonDataService = new();
        private readonly NullLogger<OrganisationRegistrationController> _nullLogger = new();
        private readonly string testDataFilePath = "API/Controllers/OrganisationRegistration/paginateddummydata.json";

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Setup()
        {
            if (TestContext.TestName.Contains("ManageModelState"))
            {

                if (TestContext.TestName.Contains("BadRequest"))
                    SetupModelStateForBadModel();
                else
                    SetupModelStateForGoodModel();
            }
            if (TestContext.TestName.Contains("Handle"))
            {
                SetupHttpMocks();
            }
        }

        [TestCategory("Modelstate")]
        [TestMethod]
        public void ManageModelState_Returns_BadRequest_When_Supplying_An_Invalid_Model_Object()
        {
            var result = _sut.ValidateIncomingModels(_badModelStateDictionary);
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestCategory("Modelstate")]
        [TestMethod]
        public void ManageModelState_Returns_Null_When_Supplying_An_Valid_Model_Object()
        {
            var result = _sut.ValidateIncomingModels(_goodModelStateDictionary);
            result.Should().BeNull();
        }

        [TestCategory("MessageHandler")]
        [TestMethod]
        public async Task When_calling_HandleGetOrganisationRegistrations_WithData_returns_200_ok_response()
        {
            //Arrange
            OrganisationRegistrationFilter request = new() { PageNumber = 1, PageSize = 2 };

            //Act
            var response = await _sut.HandleGetOrganisationRegistrations(request);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var okObjectResult = response as OkObjectResult;
            okObjectResult.StatusCode.Should().Be(200);
            okObjectResult.Value.Should().BeOfType<PaginatedResponse<OrganisationRegistrationSummaryResponse>>();
        }

        [TestMethod]
        public async Task When_calling_HandleGetOrganisationRegistrations_WithOutData_returns_500_internalservererror()
        {
            //Arrange
            OrganisationRegistrationFilter request = new() { PageNumber = 1, PageSize = 2 };

            //Act
            var response = await _sut.HandleGetOrganisationRegistrations(request);

            // Assert
            response.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = response as StatusCodeResult;
            statusCodeResult.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_OrganisationName_Is_Transferred()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { OrganisationName = "test" };

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual("test", result.OrganisationName);
        }
        
        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_OrganisationReference_Is_Transferred()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { OrganisationReference = "test" };

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual("test", result.OrganisationReference );
        }
        
        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_RegistrationYear_Is_Transferred()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { RegistrationYears = "test" };

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual("test", result.RegistrationYears);
        }
        
        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_PageSize_Is_Transferred()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { PageSize = 1 };

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual(1, result.PageSize);
        }
        
        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_PageNumber_Is_Transferred()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { PageNumber = 1 };

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual(1, result.PageNumber);
        }

        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_OrganisationType_Is_Transferred()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { OrganisationType = Facade.Core.Enums.OrganisationType.compliance };

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual("compliance", result.OrganisationType);
        }

        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_OrganisationType_Is_Not_Transferred_When_Set_to_none()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { OrganisationType = Facade.Core.Enums.OrganisationType.none};

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual("", result.OrganisationType);
        }

        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_RegistrationStatus_Is_Transferred()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { Statuses = Facade.Core.Enums.RegistrationStatus.queried };

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual("queried", result.Statuses);
        }

        [TestMethod]
        public void When_Converting_a_OrganisationRegistrationFilter_To_Request_RegistrationStatus_Is_Not_Transferred_When_Set_to_none()
        {
            OrganisationRegistrationFilter organisationRegistrationFilter = new() { Statuses = Facade.Core.Enums.RegistrationStatus.none};

            var result = (GetOrganisationRegistrationRequest)organisationRegistrationFilter;

            Assert.AreEqual("", result.Statuses);
        }

        #region Setup for ModelState tests
        private void SetupModelStateForBadModel()
        {
            var validationContext = new ValidationContext(_badParameter);
            var validationResults = new List<ValidationResult>();
            _sut = new OrganisationRegistrationHandlers(null, null);
            Validator.TryValidateObject(_badParameter, validationContext, validationResults, true);
            ApplyModelState(validationResults, _badModelStateDictionary);
        }
        private void SetupModelStateForGoodModel()
        {
            var validationContext = new ValidationContext(_goodParameter);
            var validationResults = new List<ValidationResult>();
            _sut = new OrganisationRegistrationHandlers(null, null);
            Validator.TryValidateObject(_goodParameter, validationContext, validationResults, true);
            ApplyModelState(validationResults, _badModelStateDictionary);
        }

        private void ApplyModelState(List<ValidationResult> validationResults, ModelStateDictionary modelStateDictionary)
        {
            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    modelStateDictionary.AddModelError(memberName, validationResult.ErrorMessage);
                }
            }
        }
        #endregion Setup for ModelState tests

        #region Setup HTTP Tests
        private void SetupHttpMocks()
        {
            _sut = new OrganisationRegistrationHandlers(
                _mockCommonDataService.Object,
                _nullLogger);

            if (TestContext.TestName.Contains("WithData"))
            {
                SetupHttpForMocksWithData();
            }
            else
            {
                SetupHttpMocksWithNoData();
            }
        }

        private void SetupHttpMocksWithNoData()
        {
            var handlerResponse =
                    _fixture
                        .Build<HttpResponseMessage>()
                        .With(x => x.StatusCode, HttpStatusCode.InternalServerError)
                        .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                        .Create();

            _mockCommonDataService.Setup(m => m.GetOrganisationRegistrations<JsonOrganisationRegistrationHandler>(It.IsAny<GetOrganisationRegistrationRequest>()))
                .ReturnsAsync(handlerResponse);
        }

        private void SetupHttpForMocksWithData()
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
        #endregion Setup HTTP Tests
    }
}
