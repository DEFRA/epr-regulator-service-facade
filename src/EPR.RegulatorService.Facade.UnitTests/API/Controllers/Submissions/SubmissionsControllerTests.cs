using System.Net;
using System.Text.Json;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Requests.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.PoM;
using EPR.RegulatorService.Facade.Core.Models.Responses.Submissions.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Submissions;
using EPR.RegulatorService.Facade.Core.Models.Submissions.Events;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.Messaging;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using EPR.RegulatorService.Facade.UnitTests.API.MockData;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Testing.Platform.Extensions;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.API.Controllers.Submissions
{
    [TestClass]
    public class SubmissionsControllerTests : Controller
    {
        private readonly NullLogger<SubmissionsController> _nullLogger = new();
        private readonly Mock<ISubmissionService> _mockSubmissionsService = new();
        private readonly Mock<IRegulatorOrganisationService> _mockRegulatorOrganisationService = new();
        private readonly Mock<IMessagingService> _messagingServiceMock = new();
        private readonly Mock<IRegulatorUsers> _regulatorUsersMock = new();
        private readonly Mock<ICommonDataService> _mockCommonDataService = new();
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private SubmissionsController _sut;
        private readonly Guid _oid = Guid.NewGuid();

        [TestInitialize]
        public void Setup()
        {
            var messagingConfig = Options.Create(new MessagingConfig());
            
            _sut = new SubmissionsController(
                _nullLogger, 
                _mockSubmissionsService.Object, 
                _mockRegulatorOrganisationService.Object, 
                messagingConfig, 
                _messagingServiceMock.Object,
                _mockCommonDataService.Object);
            
            _sut.AddDefaultContextWithOid(_oid, "TestAuth");
        }

        [TestMethod]
        public async Task When_creating_regulator_pom_decision_event_with_valid_data_should_return_success()
        {
            // Arrange
            var request = new RegulatorPoMDecisionCreateRequest
            {
                SubmissionId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                Decision = RegulatorDecision.Accepted
            };
            
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.NoContent)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorPoMDecisionEvent>(), It.IsAny<Guid>()))
                .ReturnsAsync(handlerResponse).Verifiable();

            _mockRegulatorOrganisationService
                .Setup(x => x.GetRegulatorUserList(It.IsAny<Guid>(), It.IsAny<Guid>(), true))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent(GetRegulatorUsers())
                });

            _regulatorUsersMock.Setup(x => x.GetRegulatorUsers(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(RegulatorUsersMockData.GetRegulatorUsers());

            // Act
            var result = await _sut.RegulatorPoMDecisionEvent(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task When_creating_regulator_pom_decision_event_with_invalid_data_should_return_400_bad_request()
        {
            // Arrange
            var request = new RegulatorPoMDecisionCreateRequest
            {
                SubmissionId = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                Decision = RegulatorDecision.Rejected
            };
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorPoMDecisionEvent>(), It.IsAny<Guid>())
            ).ReturnsAsync(handlerResponse).Verifiable();
            
            // Act
            var result = await _sut.RegulatorPoMDecisionEvent(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [TestMethod]
        public async Task When_creating_regulator_pom_decision_event_with_default_submission_id_should_return_400_bad_request()
        {
            // Arrange
            var request = new RegulatorPoMDecisionCreateRequest
            {
                SubmissionId = Guid.Empty,
                FileId = Guid.NewGuid(),
                Decision = RegulatorDecision.Rejected
            };
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorPoMDecisionEvent>(), It.IsAny<Guid>())
            ).ReturnsAsync(handlerResponse).Verifiable();

            // Act
            var result = await _sut.RegulatorPoMDecisionEvent(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [TestMethod]
        public async Task When_creating_regulator_pom_decision_event_with_default_file_id_should_return_400_bad_request()
        {
            // Arrange
            var request = new RegulatorPoMDecisionCreateRequest
            {
                SubmissionId = Guid.NewGuid(),
                FileId = Guid.Empty,
                Decision = RegulatorDecision.Rejected
            };
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorPoMDecisionEvent>(), It.IsAny<Guid>())
            ).ReturnsAsync(handlerResponse).Verifiable();

            // Act
            var result = await _sut.RegulatorPoMDecisionEvent(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [TestMethod]
        public async Task When_fetching_pom_submission_events_with_valid_data_should_return_success()
        {
            // Arrange
            var request = new GetPomSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };

            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new SubmissionEventsLastSync()))
            }).Verifiable();

            _mockSubmissionsService.Setup(x =>
                x.GetDeltaPoMSubmissions(It.IsAny<DateTime>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<RegulatorPomDecision>()))
            }).Verifiable();
            
            _mockCommonDataService.Setup(x =>
                x.GetPoMSubmissions(It.IsAny<GetPomSubmissionsRequest>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new PaginatedResponse<PomSubmissionSummaryResponse>()))
            }).Verifiable();
            
            // Act
            var result = await _sut.GetPoMSubmissions(request);

            // Assert
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task When_fetching_pom_submission_events_from_common_data_service_with_invalid_data_should_return_bad_request()
        {
            // Arrange
            var request = new GetPomSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };

            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new SubmissionEventsLastSync()))
            }).Verifiable();

            _mockSubmissionsService.Setup(x =>
                x.GetDeltaPoMSubmissions(It.IsAny<DateTime>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<RegulatorPomDecision>()))
            }).Verifiable();
            
            _mockCommonDataService.Setup(x =>
                x.GetPoMSubmissions(It.IsAny<GetPomSubmissionsRequest>())
            ).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)).Verifiable();
            
            // Act
            var result = await _sut.GetPoMSubmissions(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [TestMethod]
        public async Task When_fetching_pom_submission_events_from_common_data_service_and_last_sync_time_returns_500_should_return_internal_service_error()
        {
            // Arrange
            var request = new GetPomSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };

            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)).Verifiable();

            // Act
            var result = await _sut.GetPoMSubmissions(request);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = result as BadRequestResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }
        
        [TestMethod]
        public async Task When_fetching_pom_submission_events_from_common_data_service_and_submissions_returns_500_should_return_internal_service_error()
        {
            // Arrange
            var request = new GetPomSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };
            
            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new SubmissionEventsLastSync()))
            }).Verifiable();

            _mockSubmissionsService.Setup(x =>
                x.GetDeltaPoMSubmissions(It.IsAny<DateTime>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)).Verifiable();

            // Act
            var result = await _sut.GetPoMSubmissions(request);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = result as BadRequestResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }

        [TestMethod]
        public async Task When_fetching_registration_submissions_with_invalid_filters_will_return_validation_problem()
        {
            var model = new RegistrationSubmissionsFilters();

            _sut.ModelState.AddModelError("PageNumber", "PageNumber is required");

            var result = await _sut.GetRegistrationSubmissions(model);
            var objectResult = result as ObjectResult;

            objectResult.Value.Should().BeOfType<ValidationProblemDetails>();
        }

        [TestMethod]
        public async Task When_fetching_registration_submission_events_with_valid_data_should_return_success()
        {
            // Arrange
            var request = new GetRegistrationSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };

            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new SubmissionEventsLastSync()))
            }).Verifiable();

            _mockSubmissionsService.Setup(x =>
                x.GetDeltaRegistrationSubmissions(It.IsAny<DateTime>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<RegulatorRegistrationDecision>()))
            }).Verifiable();
            
            _mockCommonDataService.Setup(x =>
                x.GetRegistrationSubmissions(It.IsAny<GetRegistrationSubmissionsRequest>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new PaginatedResponse<PomSubmissionSummaryResponse>()))
            }).Verifiable();
            
            // Act
            var result = await _sut.GetRegistrationSubmissions(request);

            // Assert
            var statusCodeResult = result as OkObjectResult;
            statusCodeResult?.StatusCode.Should().Be(200);
        }
        
        [TestMethod]
        public async Task When_fetching_registration_submission_events_from_common_data_service_with_invalid_data_should_return_bad_request()
        {
            // Arrange
            var request = new GetRegistrationSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };

            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new SubmissionEventsLastSync()))
            }).Verifiable();

            _mockSubmissionsService.Setup(x =>
                x.GetDeltaRegistrationSubmissions(It.IsAny<DateTime>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<RegulatorRegistrationDecision>()))
            }).Verifiable();
            
            _mockCommonDataService.Setup(x =>
                x.GetRegistrationSubmissions(It.IsAny<GetRegistrationSubmissionsRequest>())
            ).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.BadRequest)).Verifiable();
            
            // Act
            var result = await _sut.GetRegistrationSubmissions(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [TestMethod]
        public async Task When_fetching_registration_submission_events_from_common_data_service_and_last_sync_time_returns_500_should_return_internal_service_error()
        {
            // Arrange
            var request = new GetRegistrationSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };

            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)).Verifiable();

            // Act
            var result = await _sut.GetRegistrationSubmissions(request);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = result as BadRequestResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }
        
        [TestMethod]
        public async Task When_fetching_registration_submission_events_from_common_data_service_and_submissions_returns_500_should_return_internal_service_error()
        {
            // Arrange
            var request = new GetRegistrationSubmissionsRequest
            {
                PageNumber = 1,
                PageSize = 20
            };
            
            _mockCommonDataService.Setup(x =>
                x.GetSubmissionLastSyncTime()
            ).ReturnsAsync(new HttpResponseMessage()
            {
                Content = new StringContent(JsonSerializer.Serialize(new SubmissionEventsLastSync()))
            }).Verifiable();

            _mockSubmissionsService.Setup(x =>
                x.GetDeltaRegistrationSubmissions(It.IsAny<DateTime>(), It.IsAny<Guid>())
            ).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError)).Verifiable();

            // Act
            var result = await _sut.GetRegistrationSubmissions(request);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var statusCodeResult = result as BadRequestResult;
            statusCodeResult?.StatusCode.Should().Be(500);
        }
        
        private static string GetRegulatorUsers()
        {
            return JsonSerializer.Serialize(RegulatorUsersMockData.GetRegulatorUsers());
        }
        
        [TestMethod]
        public async Task When_creating_regulator_registration_decision_event_with_valid_data_for_acceptance_should_return_success()
        {
            // Arrange
            var request = new RegulatorRegistrationDecisionCreateRequest
            {
                SubmissionId = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                Decision = RegulatorDecision.Accepted
            };
            
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.NoContent)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                    x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorRegistrationDecisionEvent>(), It.IsAny<Guid>()))
                .ReturnsAsync(handlerResponse).Verifiable();

            _mockRegulatorOrganisationService
                .Setup(x => x.GetRegulatorUserList(It.IsAny<Guid>(), It.IsAny<Guid>(), true))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent(GetRegulatorUsers())
                });

            _regulatorUsersMock.Setup(x => x.GetRegulatorUsers(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(RegulatorUsersMockData.GetRegulatorUsers());

            // Act
            var result = await _sut.CreateRegulatorRegistrationDecisionEvent(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task When_creating_regulator_registration_decision_event_with_valid_data_for_rejection_should_return_success()
        {
            // Arrange
            var request = new RegulatorRegistrationDecisionCreateRequest
            {
                SubmissionId = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                Decision = RegulatorDecision.Rejected
            };
            
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.NoContent)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                    x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorRegistrationDecisionEvent>(), It.IsAny<Guid>()))
                .ReturnsAsync(handlerResponse).Verifiable();

            _mockRegulatorOrganisationService
                .Setup(x => x.GetRegulatorUserList(It.IsAny<Guid>(), It.IsAny<Guid>(), true))
                .ReturnsAsync(new HttpResponseMessage()
                {
                    Content = new StringContent(GetRegulatorUsers())
                });

            _regulatorUsersMock.Setup(x => x.GetRegulatorUsers(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(RegulatorUsersMockData.GetRegulatorUsers());

            // Act
            var result = await _sut.CreateRegulatorRegistrationDecisionEvent(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
        
        [TestMethod]
        public async Task When_creating_regulator_registration_decision_event_with_invalid_data_should_return_400_bad_request()
        {
            // Arrange
            var request = new RegulatorRegistrationDecisionCreateRequest
            {
                SubmissionId = Guid.NewGuid(),
                FileId = Guid.NewGuid(),
                Decision = RegulatorDecision.Rejected
            };
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorRegistrationDecisionEvent>(), It.IsAny<Guid>())
            ).ReturnsAsync(handlerResponse).Verifiable();
            
            // Act
            var result = await _sut.CreateRegulatorRegistrationDecisionEvent(request);

            // Assert
            result.Should().BeOfType<BadRequestResult>();
        }
        
        [TestMethod]
        public async Task When_creating_regulator_registration_decision_event_which_invalidates_model_should_return_400_bad_request()
        {
            // Arrange
            var request = new RegulatorRegistrationDecisionCreateRequest
            {
                SubmissionId = Guid.Empty,
                FileId = Guid.Empty,
                Decision = RegulatorDecision.Rejected
            };
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

            _mockSubmissionsService.Setup(x =>
                x.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegulatorRegistrationDecisionEvent>(), It.IsAny<Guid>())
            ).ReturnsAsync(handlerResponse).Verifiable();

            _sut.ModelState.AddModelError(nameof(RegulatorRegistrationDecisionCreateRequest.SubmissionId),
                string.Empty);

            // Act
            var result = await _sut.CreateRegulatorRegistrationDecisionEvent(request);
            
            // Assert
            result.As<ObjectResult>().Value.Should().BeOfType<ValidationProblemDetails>();
        }

        [TestMethod]
        public async Task When_Requesting_Pom_Resubmission_Paycal_details_Without_SubmissionId_Will_Return_BadRequest()
        {
            // Arrange
            _sut.ModelState.AddModelError("SubmissionId", "Invalid");

            // Act
            var result = await _sut.GetResubmissionPaycalDetails(Guid.Empty, null);

            // Assert
            result.As<ObjectResult>().Value.Should().BeOfType<ValidationProblemDetails>();
        }

        [TestMethod]
        public async Task When_Requesting_Pom_Resubmission_Paycal_Details_Will_Throw_428_When_No_ReferenceNumber_Available_And_IsResubmission_IsTrue()
        {
            PomResubmissionPaycalParametersDto returnObj = new PomResubmissionPaycalParametersDto { IsResubmission = true, Reference = null };

            // Arrange
            _mockCommonDataService
                .Setup(x => x.GetPomResubmissionPaycalDetails(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync((PomResubmissionPaycalParametersDto)returnObj);

            // Act
            var result = await _sut.GetResubmissionPaycalDetails(Guid.NewGuid(), null);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult).StatusCode.Should().Be(428);
        }

        [TestMethod]
        public async Task When_Requesting_Pom_Resubmission_Paycal_Details_Will_Return_OK_When_No_ReferenceNumber_Available_And_IsResubmission_IsFalse()
        {
            PomResubmissionPaycalParametersDto returnObj = new PomResubmissionPaycalParametersDto { IsResubmission = false, Reference = null };

            // Arrange
            _mockCommonDataService
                .Setup(x => x.GetPomResubmissionPaycalDetails(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync((PomResubmissionPaycalParametersDto)returnObj);

            // Act
            var result = await _sut.GetResubmissionPaycalDetails(Guid.NewGuid(), null);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task When_Requesting_Pom_Resubmission_Paycal_Details_With_Valid_Ids_Will_Return_Ok()
        {
            // Arrange
            var response = _fixture.Create<PomResubmissionPaycalParametersDto>();
            _mockCommonDataService
                .Setup(x => x.GetPomResubmissionPaycalDetails(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ReturnsAsync(response);

            // Act
            var result = await _sut.GetResubmissionPaycalDetails(Guid.NewGuid(), Guid.NewGuid());

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod]
        public async Task When_Service_Throws_Exception_Will_Return_InternalServerError()
        {
            // Arrange
            _mockCommonDataService
                .Setup(x => x.GetPomResubmissionPaycalDetails(It.IsAny<Guid>(), It.IsAny<Guid?>()))
                .ThrowsAsync(new Exception("Service failure"));

            // Act
            Func<Task> act = async () => await _sut.GetResubmissionPaycalDetails(Guid.NewGuid(), null);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Service failure");
        }
    }
}