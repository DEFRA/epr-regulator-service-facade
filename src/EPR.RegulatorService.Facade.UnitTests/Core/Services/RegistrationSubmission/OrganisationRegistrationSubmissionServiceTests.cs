using AutoFixture.AutoMoq;
using AutoFixture;
using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.Responses.OrganisationRegistrations;
using EPR.RegulatorService.Facade.Core.Models.Submissions;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using static EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission.OrganisationRegistrationSubmissionService;
using EPR.RegulatorService.Facade.Core.Models.Responses.RegistrationSubmissions;
using EPR.RegulatorService.Facade.Core.Models.RegistrationSubmissions;
using FluentAssertions;
using System.Net;
using System.Runtime.CompilerServices;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.RegistrationSubmission;

[TestClass]
public class OrganisationRegistrationSubmissionServiceTests
{
    private readonly Mock<ISubmissionService> _submissionsServiceMock = new();
    private readonly Mock<ICommonDataService> _commonDataServiceMock = new();
    private OrganisationRegistrationSubmissionService _sut;
    private IFixture _fixture;

    [TestInitialize]
    public void Setup()
    {
        _sut = new OrganisationRegistrationSubmissionService(_commonDataServiceMock.Object,
                                                             _submissionsServiceMock.Object,
                                                             new Mock<ILogger<OrganisationRegistrationSubmissionService>>().Object);

        _fixture = new Fixture().Customize(new AutoMoqCustomization());
    }

    [TestMethod]
    public async Task Should_return_valid_ReferenceNumber()
    {
        //Arrange  
        string year = "99";
        string orgId = "123456";
        string appRefNumber = string.Empty;

        // Act 
        var result = _sut.GenerateReferenceNumber(
            CountryName.Eng,
            RegistrationSubmissionType.Producer,
            appRefNumber,
            orgId,
            year);

        // Assert  
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length == 15);
        Assert.IsTrue(result.StartsWith('R'));
        Assert.IsTrue(result.StartsWith($"R{year}"));
        Assert.IsTrue(result.StartsWith($"R{year}EP"));
    }

    [TestMethod]
    public async Task Should_return_valid_ReferenceNumber_ForAComplianceScheme()
    {
        //Arrange  
        string year = "99";
        string orgId = "123456";
        string appRefNumber = "PEPR00000123425C1";

        // Act 
        var result = _sut.GenerateReferenceNumber(
            CountryName.Eng,
            RegistrationSubmissionType.ComplianceScheme,
            appRefNumber,
            orgId,
            year);

        // Assert  
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length == 17);
        Assert.IsTrue(result.StartsWith('R'));
        Assert.IsTrue(result.StartsWith($"R{year}"));
        Assert.IsTrue(result.StartsWith($"R{year}EC"));
        Assert.IsTrue(result.StartsWith($"R{year}EC{orgId}234"));
    }

    [TestMethod]
    public async Task Should_Return_GetRegistrationSubmissionList()
    {
        // Arrage

        var filterRequest = new GetOrganisationRegistrationSubmissionsFilter
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            OrganisationType = "LARGE",

        };

        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(filterRequest))
            .ReturnsAsync(new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>()).Verifiable();

        //Act
        var result = _sut.HandleGetRegistrationSubmissionList(filterRequest, Guid.NewGuid());

        //Assert
        Assert.IsNotNull(result);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionList(filterRequest), Times.AtMostOnce);
    }

    [TestMethod]
    public async Task Should_Return_GetRegistrationSubmissionList_With_lastSyncTime_True()
    {
        // Arrage
        var submissionId = Guid.NewGuid();

        var filterRequest = new GetOrganisationRegistrationSubmissionsFilter
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            OrganisationType = "LARGE",

        };

        var response = new RegistrationSubmissionOrganisationDetailsResponse
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            ApplicationReferenceNumber = "APPREF123",
            RegistrationReferenceNumber = "REGREF456",
            OrganisationType = RegistrationSubmissionOrganisationType.small

        };

        var submissionEventsLastSync = _fixture.Build<SubmissionEventsLastSync>().Create();

        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(filterRequest))
            .ReturnsAsync(new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>()).Verifiable();

        var submissionLastSyncTimeResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(submissionEventsLastSync))
        };

        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisions = [];


        List<AbstractCosmosSubmissionEvent> updateDeltaRegistrationDecisions = deltaRegistrationDecisions.Select(x => new AbstractCosmosSubmissionEvent
        {
            AppReferenceNumber = response.ApplicationReferenceNumber,
            RegistrationReferenceNumber = response.RegistrationReferenceNumber,
            Comments = x.Comments,
            Created = x.Created,
            Decision = x.Decision,
            DecisionDate = x.DecisionDate,
            SubmissionId = submissionId,
            Type = x.Type
        }).ToList();


        var deltaRegistrationDecisionsResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(updateDeltaRegistrationDecisions))

        };

        _commonDataServiceMock.Setup(x => x.GetSubmissionLastSyncTime()).ReturnsAsync(submissionLastSyncTimeResponse);
        _submissionsServiceMock.Setup(x => x.GetDeltaOrganisationRegistrationEvents(It.IsAny<DateTime>(), It.IsAny<Guid>(), null))
            .ReturnsAsync(deltaRegistrationDecisionsResponse);

        //Act
        var result = _sut.HandleGetRegistrationSubmissionList(filterRequest, Guid.NewGuid());


        //Assert
        Assert.IsNotNull(result);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionList(filterRequest), Times.AtMostOnce);
        _commonDataServiceMock.Verify(x => x.GetSubmissionLastSyncTime(), Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task Should_Return_GetRegistrationSubmissionList_With_ApplyAppRefNumbers()
    {
        // Arrage
        var submissionId = Guid.NewGuid();

        var filterRequest = new GetOrganisationRegistrationSubmissionsFilter
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            OrganisationType = "LARGE",
            Statuses = "Test Status"
        };

        var response = new RegistrationSubmissionOrganisationDetailsResponse
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            ApplicationReferenceNumber = "APPREF123",
            RegistrationReferenceNumber = "REGREF456",
            OrganisationType = RegistrationSubmissionOrganisationType.small

        };

        var submissionEventsLastSync = _fixture.Build<SubmissionEventsLastSync>().Create();

        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(filterRequest))
            .ReturnsAsync(new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>()).Verifiable();

        var submissionLastSyncTimeResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(submissionEventsLastSync))
        };

        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisions = Enumerable.Range(0, 20)
                                                                        .Select(x => _fixture.Build<AbstractCosmosSubmissionEvent>().Create())
                                                                        .ToList();

        PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse> organisationRegistrationSubmissionSummaryResponse = new()
        {
            pageSize = 20,
            currentPage = 1,
            items = [],
            totalItems = 0
        };


        List<AbstractCosmosSubmissionEvent> updateDeltaRegistrationDecisions = deltaRegistrationDecisions.Select(x => new AbstractCosmosSubmissionEvent
        {
            AppReferenceNumber = response.ApplicationReferenceNumber,
            RegistrationReferenceNumber = response.RegistrationReferenceNumber,
            Comments = x.Comments,
            Created = x.Created,
            Decision = x.Decision,
            DecisionDate = x.DecisionDate,
            SubmissionId = submissionId,
            Type = x.Type
        }).ToList();


        var deltaRegistrationDecisionsResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(updateDeltaRegistrationDecisions))

        };

        _commonDataServiceMock.Setup(x => x.GetSubmissionLastSyncTime()).ReturnsAsync(submissionLastSyncTimeResponse);
        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionList(It.IsAny<GetOrganisationRegistrationSubmissionsFilter>())).ReturnsAsync(organisationRegistrationSubmissionSummaryResponse);
        _submissionsServiceMock.Setup(x => x.GetDeltaOrganisationRegistrationEvents(It.IsAny<DateTime>(), It.IsAny<Guid>(), null))
            .ReturnsAsync(deltaRegistrationDecisionsResponse);

        //Act
        var result = _sut.HandleGetRegistrationSubmissionList(filterRequest, Guid.NewGuid());


        //Assert
        Assert.IsNotNull(result);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionList(filterRequest), Times.AtMostOnce);
        _commonDataServiceMock.Verify(x => x.GetSubmissionLastSyncTime(), Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task Should_Return_GetOrganisationRegistrationSubmissionDetails()
    {
        // Arrage

        var submissionId = Guid.NewGuid();

        var response = new RegistrationSubmissionOrganisationDetailsResponse
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            ApplicationReferenceNumber = "APPREF123",
            RegistrationReferenceNumber = "REGREF456",
            OrganisationType = RegistrationSubmissionOrganisationType.small

        };

        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionDetails(submissionId))
            .ReturnsAsync(response).Verifiable();

        //Act
        var result = _sut.HandleGetOrganisationRegistrationSubmissionDetails(submissionId, Guid.NewGuid());

        //Assert
        Assert.IsNotNull(result);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionDetails(submissionId), Times.AtMostOnce);
        _submissionsServiceMock.Verify(x => x.GetDeltaOrganisationRegistrationEvents(It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }


    [TestMethod]
    public async Task Should_Return_GetOrganisationRegistrationSubmissionDetails_With_lastSyncTime_True()
    {
        // Arrage

        var submissionId = Guid.NewGuid();

        var response = new RegistrationSubmissionOrganisationDetailsResponse
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            ApplicationReferenceNumber = "APPREF123",
            RegistrationReferenceNumber = "REGREF456",
            OrganisationType = RegistrationSubmissionOrganisationType.small

        };

        var submissionEventsLastSync = _fixture.Build<SubmissionEventsLastSync>().Create();

        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisions = Enumerable.Range(0, 20)
                                                                        .Select(x => _fixture.Build<AbstractCosmosSubmissionEvent>().Create())
                                                                        .ToList();

        var submissionLastSyncTimeResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(submissionEventsLastSync))

        };

        var deltaRegistrationDecisionsResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(deltaRegistrationDecisions))

        };


        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionDetails(submissionId))
            .ReturnsAsync(response).Verifiable();
        _commonDataServiceMock.Setup(x => x.GetSubmissionLastSyncTime()).ReturnsAsync(submissionLastSyncTimeResponse);
        _submissionsServiceMock.Setup(x => x.GetDeltaOrganisationRegistrationEvents(It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(deltaRegistrationDecisionsResponse);


        //Act
        var result = _sut.HandleGetOrganisationRegistrationSubmissionDetails(submissionId, Guid.NewGuid());

        //Assert
        Assert.IsNotNull(result);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionDetails(submissionId), Times.AtMostOnce);
        _submissionsServiceMock.Verify(x => x.GetDeltaOrganisationRegistrationEvents(It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.AtLeastOnce);
    }


    [TestMethod]
    [DataRow("RegulatorRegistrationDecision")]
    [DataRow("RegistrationApplicationSubmitted")]
    public async Task Should_Return_GetOrganisationRegistrationSubmissionDetails_And_Assign_RegulatorDetails(string type)
    {
        // Arrage

        var submissionId = Guid.NewGuid();

        var response = new RegistrationSubmissionOrganisationDetailsResponse
        {

            OrganisationReference = "ORGREF1234567890",
            OrganisationName = "Test Organisation",
            ApplicationReferenceNumber = "APPREF123",
            RegistrationReferenceNumber = "REGREF456",
            OrganisationType = RegistrationSubmissionOrganisationType.small

        };

        var submissionEventsLastSync = _fixture.Build<SubmissionEventsLastSync>().Create();

        List<AbstractCosmosSubmissionEvent> deltaRegistrationDecisions = Enumerable.Range(0, 20)
                                                                        .Select(x => _fixture.Build<AbstractCosmosSubmissionEvent>().Create())
                                                                        .ToList();


        List<AbstractCosmosSubmissionEvent> updateDeltaRegistrationDecisions = deltaRegistrationDecisions.Select(x => new AbstractCosmosSubmissionEvent
        {
            AppReferenceNumber = response.ApplicationReferenceNumber,
            RegistrationReferenceNumber = response.RegistrationReferenceNumber,
            Comments = x.Comments,
            Created = x.Created,
            Decision = x.Decision,
            DecisionDate = x.DecisionDate,
            SubmissionId = submissionId,
            Type = type
        }).ToList();
        var submissionLastSyncTimeResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(submissionEventsLastSync))

        };

        var deltaRegistrationDecisionsResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(updateDeltaRegistrationDecisions))

        };

        _commonDataServiceMock.Setup(x => x.GetOrganisationRegistrationSubmissionDetails(submissionId))
            .ReturnsAsync(response).Verifiable();
        _commonDataServiceMock.Setup(x => x.GetSubmissionLastSyncTime()).ReturnsAsync(submissionLastSyncTimeResponse);
        _submissionsServiceMock.Setup(x => x.GetDeltaOrganisationRegistrationEvents(It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(deltaRegistrationDecisionsResponse);


        //Act
        var result = _sut.HandleGetOrganisationRegistrationSubmissionDetails(submissionId, Guid.NewGuid());

        //Assert
        Assert.IsNotNull(result);
        _commonDataServiceMock.Verify(r => r.GetOrganisationRegistrationSubmissionDetails(submissionId), Times.AtMostOnce);
        _submissionsServiceMock.Verify(x => x.GetDeltaOrganisationRegistrationEvents(It.IsAny<DateTime>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task Should_throw_with_null_year()
    {
        //Arrange  
        // Act
        Assert.ThrowsException<ArgumentNullException>(() => 
        _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Producer, string.Empty, "123456"));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow(null)]
    public async Task Should_Throw_With_Incorrect_OrganisationId(string organisationId)
    {
        //Arrange  
        // Act
        Assert.ThrowsException<ArgumentNullException>(() =>
        _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Producer, string.Empty, organisationId, "24"));
    }

    [TestMethod]
    public async Task Should_return_valid_ReferenceNumber_For_Exporter_With_Steel()
    {
        //Arrange  
        string year = (DateTime.Now.Year % 100).ToString("D2");
        string orgId = "123456";
        string appRefNumber = string.Empty;

        // Act 
        var result = _sut.GenerateReferenceNumber(CountryName.Eng,
            RegistrationSubmissionType.Exporter,
            appRefNumber,
            orgId,
            year,
            MaterialType.Steel);

        // Assert  
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length == 17);
        Assert.IsTrue(result.StartsWith('R'));
        Assert.IsTrue(result.StartsWith($"R{year}"));
        Assert.IsTrue(result.StartsWith($"R{year}EE"));
        Assert.IsTrue(result.EndsWith($"ST"));
    }

    [TestMethod]
    public async Task Should_return_valid_ReferenceNumber_For_Reprocessor_With_Plastic()
    {
        //Arrange  
        string year = (DateTime.Now.Year % 100).ToString("D2");
        string orgId = "123456";
        string appRefNumber = string.Empty;

        // Act 
        var result = _sut.GenerateReferenceNumber(
            CountryName.Eng,
            RegistrationSubmissionType.Reprocessor,
            appRefNumber,
            orgId,
            year,
            MaterialType.Plastic);

        // Assert  
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length == 17);
        Assert.IsTrue(result.StartsWith('R'));
        Assert.IsTrue(result.StartsWith($"R{year}"));
        Assert.IsTrue(result.StartsWith($"R{year}ER"));
        Assert.IsTrue(result.EndsWith($"PL"));
    }

    [TestMethod]
    public async Task Should_throw_exception_if_OrgId_IsNull()
    {
        // Act 
        Assert.ThrowsException<ArgumentNullException>(() => _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Producer, string.Empty, null));
    }

    [TestMethod]
    public void NoMatchingCosmosItems_NoChanges()
    {
        // Arrange
        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items = new List<OrganisationRegistrationSubmissionSummaryResponse>
                {
                    new() {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = null,
                        SubmissionStatus = RegistrationSubmissionStatus.Pending
                    }
                }
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                // No event matches "APP123"
                new() {
                    AppReferenceNumber = "DIFFERENT_APP",
                    Created = DateTime.UtcNow,
                    Type = "RegulatorRegistrationDecision",
                    Decision = "Granted"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.IsNull(item.RegulatorDecisionDate, "No matching events means RegulatorCommentDate remains null.");
        Assert.AreEqual(RegistrationSubmissionStatus.Pending, item.SubmissionStatus, "SubmissionStatus should remain unchanged.");
    }

    [TestMethod]
    public void RegulatorDecisionUpdatesWhenRegulatorCommentDateNull()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items = new List<OrganisationRegistrationSubmissionSummaryResponse>
                {
                    new() {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = null,
                        SubmissionStatus = RegistrationSubmissionStatus.Pending,
                        RegistrationReferenceNumber = "OLD_REF"
                    }
                }
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "app123", // case-insensitive match
                    Created = now,
                    Decision = "Granted",
                    DecisionDate = now.AddDays(-1),
                    RegistrationReferenceNumber = "NEW_REF",
                    Type = "regulatorregistrationdecision"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.AreEqual(now, item.RegulatorDecisionDate, "RegulatorCommentDate should update to event's Created.");
        Assert.AreEqual("NEW_REF", item.RegistrationReferenceNumber, "RegistrationReferenceNumber should update from event.");
        Assert.AreEqual(now.AddDays(-1), item.StatusPendingDate, "StatusPendingDate should match DecisionDate from event.");
        Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus, "SubmissionStatus should match event's Decision.");
    }

    [TestMethod]
    public void RegulatorDecisionUpdatesOnlyIfNewerDate()
    {
        // Arrange
        var oldDate = DateTime.UtcNow.AddDays(-2);
        var existingDate = DateTime.UtcNow.AddDays(-1);
        var newDate = DateTime.UtcNow;

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items = new List<OrganisationRegistrationSubmissionSummaryResponse>
                {
                    new() {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = existingDate,
                        SubmissionStatus = RegistrationSubmissionStatus.Pending,
                        RegistrationReferenceNumber = "OLD_REF"
                    }
                }
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                // This event is older than existingDate, so should not update
                new() {
                    AppReferenceNumber = "APP123",
                    Created = oldDate,
                    Decision = "Refused",
                    DecisionDate = oldDate,
                    RegistrationReferenceNumber = "SHOULD_NOT_UPDATE",
                    Type = "RegulatorRegistrationDecision"
                },
                // This event is newer and should update
                new() {
                    AppReferenceNumber = "APP123",
                    Created = newDate,
                    Decision = "Cancelled",
                    DecisionDate = newDate,
                    RegistrationReferenceNumber = "NEW_REF",
                    Type = "RegulatorRegistrationDecision"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        // The older event should not have changed anything
        // The newer event should take precedence
        Assert.AreEqual(newDate, item.RegulatorDecisionDate, "Should update to the newer event's Created date.");
        Assert.AreEqual("NEW_REF", item.RegistrationReferenceNumber, "Should update to the newer event's RegistrationReferenceNumber.");
        Assert.AreEqual(RegistrationSubmissionStatus.Cancelled, item.SubmissionStatus, "Should update to the newer event's Decision.");
    }

    [TestMethod]
    public void ProducerCommentsWhenRegulatorCommentDateNewerThanProducer()
    {
        // Arrange
        var regulatorDate = DateTime.UtcNow;
        var producerDate = DateTime.UtcNow.AddHours(-1); // older comment

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items = new List<OrganisationRegistrationSubmissionSummaryResponse>
                {
                    new() {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = regulatorDate,
                        SubmissionDate = producerDate,
                        SubmissionStatus = RegistrationSubmissionStatus.Granted
                    }
                }
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                // Producer comment is older than Regulator comment, no status update
                new() {
                    AppReferenceNumber = "APP123",
                    Created = producerDate,
                    Type = "RegistrationApplicationSubmitted"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.AreEqual(producerDate, item.SubmissionDate, "Should set SubmissionDate to the event's Created date.");
        Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus, "No status update as SubmissionDate < RegulatorDecisionDate.");
    }

    [TestMethod]
    public void MultipleRegulatorDecisionsTakeTheLatestOne()
    {
        // Arrange
        var earlier = DateTime.UtcNow.AddDays(-1);
        var later = DateTime.UtcNow;

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items = new List<OrganisationRegistrationSubmissionSummaryResponse>
                {
                    new() {
                        ApplicationReferenceNumber = "APP123",
                        SubmissionStatus = RegistrationSubmissionStatus.Pending
                    }
                }
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Created = earlier,
                    Decision = "Queried",
                    DecisionDate = earlier,
                    RegistrationReferenceNumber = "REF1",
                    Type = "RegulatorRegistrationDecision"
                },
                new() {
                    AppReferenceNumber = "APP123",
                    Created = later,
                    Decision = "Refused",
                    DecisionDate = later,
                    RegistrationReferenceNumber = "REF2",
                    Type = "RegulatorRegistrationDecision"
                }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        // Should reflect the later event
        Assert.AreEqual(later, item.RegulatorDecisionDate, "Should use the later event's Created date.");
        Assert.AreEqual("REF2", item.RegistrationReferenceNumber, "Should use the later event's RegistrationReferenceNumber.");
        Assert.AreEqual(RegistrationSubmissionStatus.Refused, item.SubmissionStatus, "Should reflect the later event's Decision.");
    }

    [Ignore("Logic to indicate Resubmission from Cosmos is invalid as the Resubmitted data may not be avialable in Synapse")]
    [TestMethod]
    public void MultipleProducerRegistrationsUpdatesResubmissionStatus()
    {
        // Arrange
        var olderComment = DateTime.UtcNow.AddHours(-1);
        var newerComment = DateTime.UtcNow.AddHours(+1);
        var regulatorDate = DateTime.UtcNow;

        var requestedList = new PaginatedResponse<OrganisationRegistrationSubmissionSummaryResponse>
        {
            items = new List<OrganisationRegistrationSubmissionSummaryResponse>
                {
                    new() {
                        ApplicationReferenceNumber = "APP123",
                        RegulatorDecisionDate = regulatorDate,
                        SubmissionStatus = RegistrationSubmissionStatus.Granted
                    }
                }
        };

        var deltaRegistrationDecisionsResponse = new List<AbstractCosmosSubmissionEvent>
            {
                new() { AppReferenceNumber = "APP123", Created = olderComment, Type = "RegistrationApplicationSubmitted" },
                new() { AppReferenceNumber = "APP123", Created = newerComment, Type = "RegistrationApplicationSubmitted" }
            };

        // Act
        MergeCosmosUpdates(deltaRegistrationDecisionsResponse, requestedList);

        // Assert
        var item = requestedList.items[0];
        Assert.IsNotNull(item);
        item.IsResubmission.Should().BeTrue();
        Assert.AreEqual(newerComment, item.ResubmissionDate, "Should have the last (newest) ResubmissionDate.");
        Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus, "Should remain Granted as newer SubmissionDate still older than RegulatorDecisionDate.");
    }

    [TestMethod]
    public async Task ShouldCreateRegulatorDecisionSubmissionEvent()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new RegulatorDecisionCreateRequest
        {
            SubmissionId = submissionId, Status = RegistrationSubmissionStatus.Granted,
            ApplicationReferenceNumber = "PEPR12345678", CountryName = CountryName.Eng,
            RegistrationSubmissionType = RegistrationSubmissionType.Producer,
            OrganisationAccountManagementId = "123456", TwoDigitYear = "25"
        };
        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()))
            .ReturnsAsync(handlerResponse)
            .Callback<Guid, RegistrationSubmissionDecisionEvent, Guid>((subId, reqEvent, userId) => {
                reqEvent.RegistrationReferenceNumber.Should().StartWith("R25EP123456");
            });

        // Act
        var result = await _sut.HandleCreateRegulatorDecisionSubmissionEvent(request, userId);

        // Assert
        Assert.IsNotNull(result);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [TestMethod]
    public async Task ShouldCreateRegulatorDecisionSubmissionEventForResubmission()
    {
        // Arrange
        var submissionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new RegulatorDecisionCreateRequest
        {
            SubmissionId = submissionId,
            Status = RegistrationSubmissionStatus.Granted,
            IsResubmission = true,
            ExistingRegRefNumber = "R25EP123456"
        };
        var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.Created)
                    .With(x => x.Content, new StringContent(_fixture.Create<string>()))
                    .Create();

        _submissionsServiceMock.Setup(r => r.CreateSubmissionEvent(It.IsAny<Guid>(), It.IsAny<RegistrationSubmissionDecisionEvent>(), It.IsAny<Guid>()))
            .ReturnsAsync(handlerResponse)
            .Callback<Guid, RegistrationSubmissionDecisionEvent, Guid>((subId, reqEvent, userId) => {
                reqEvent.RegistrationReferenceNumber.Should().StartWith("R25EP123456");
            });

        // Act
        var result = await _sut.HandleCreateRegulatorDecisionSubmissionEvent(request, userId);

        // Assert
        Assert.IsNotNull(result);
        result.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [TestClass]
    public class MergeSingleItemCosmosUpdatesTests
    {
        [TestMethod]
        public void NoMatchingEvents_NoChangesToItem()
        {
            // Arrange
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Pending);
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() { AppReferenceNumber = "DIFFERENT", Type = "RegulatorRegistrationDecision", Decision = "Granted", Created = DateTime.UtcNow }
            };

            // Act
            MergeCosmosUpdates(events, item);

            // Assert
            Assert.AreEqual(RegistrationSubmissionStatus.Pending, item.SubmissionStatus, "Status should remain unchanged.");
            Assert.IsNull(item.RegulatorDecisionDate);
            Assert.IsNull(item.ProducerCommentDate);
        }

        [TestMethod]
        public void SingleRegulatorDecisionEvent_UpdatesItemFields()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Pending);
            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "app123", // case-insensitive match
                    Type = "regulatorregistrationdecision",
                    Decision = "Granted",
                    Created = now,
                    DecisionDate = now.AddHours(-1),
                    RegistrationReferenceNumber = "NEW_REF"
                }
            };

            // Act
            MergeCosmosUpdates(events, item);

            // Assert
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus);
            Assert.AreEqual(now, item.RegulatorDecisionDate, "RegulatorDecisionDate should be updated from the event.");
            Assert.AreEqual("NEW_REF", item.RegistrationReferenceNumber, "RegistrationReferenceNumber should be updated.");
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionDetails.Status, "SubmissionDetails.Status matches SubmissionStatus.");
        }

        [TestMethod]
        public void RegulatorDecisionWithOlderDateDoesNotOverrideNewerOne()
        {
            // Arrange
            var older = DateTime.UtcNow.AddDays(-2);
            var newer = DateTime.UtcNow.AddDays(-1);
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Pending);
            // Assign a newer decision first
            var initialEvents = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123", Type = "RegulatorRegistrationDecision",
                    Decision = "Refused", Created = newer, DecisionDate = newer, RegistrationReferenceNumber = "REF1"
                }
            };
            MergeCosmosUpdates(initialEvents, item);

            // Now try with an older decision
            var olderEvent = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123", Type = "RegulatorRegistrationDecision",
                    Decision = "Granted", Created = older, DecisionDate = older, RegistrationReferenceNumber = "OLD_REF"
                }
            };

            // Act again
            MergeCosmosUpdates(olderEvent, item);

            // Assert - older should not override newer
            Assert.AreEqual(RegistrationSubmissionStatus.Refused, item.SubmissionStatus, "Should remain as the newer (Refused) decision.");
            Assert.AreEqual("REF1", item.RegistrationReferenceNumber, "Should remain as newer reference.");
            Assert.AreEqual(newer, item.RegulatorDecisionDate, "Should remain the newer date.");
        }

        [TestMethod]
        public void ProducerSubmissionUpdatesProducerCommentDate()
        {
            // Arrange
            var commentTime = DateTime.UtcNow;
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Pending);

            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegistrationApplicationSubmitted",
                    Created = commentTime,
                    Comments = "Producer Comment"
                }
            };

            // Act
            MergeCosmosUpdates(events, item);

            // Assert
            Assert.AreEqual(commentTime, item.ProducerCommentDate, "ProducerCommentDate should be updated.");
            Assert.AreEqual("Producer Comment", item.ProducerComments, "Comments should be updated.");
            // Since RegulatorDecisionDate is null, no final update check applies.
            Assert.AreEqual(RegistrationSubmissionStatus.Pending, item.SubmissionStatus);
        }

        [TestMethod]
        public void MultipleProducerComments_TakesLatestProducerCommentDate()
        {
            // Arrange
            var olderComment = DateTime.UtcNow.AddHours(-2);
            var newerComment = DateTime.UtcNow.AddHours(-1);
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Pending);

            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegistrationApplicationSubmitted",
                    Created = olderComment,
                    Comments = "Old comment"
                },
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegistrationApplicationSubmitted",
                    Created = newerComment,
                    Comments = "New comment"
                }
            };

            // Act
            MergeCosmosUpdates(events, item);

            // Assert
            Assert.AreEqual(newerComment, item.ProducerCommentDate, "Should have the newest ProducerCommentDate.");
            Assert.AreEqual("New comment", item.ProducerComments, "Should have the latest comment.");
        }

        [TestMethod]
        public void ProducerAndRegulatorDecisions_ProducerLater_Doesnt_UpdatesToUpdatedStatus()
        {
            // Arrange
            var regulatorTime = DateTime.UtcNow;
            var producerTime = DateTime.UtcNow.AddMinutes(10);
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Granted);

            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegulatorRegistrationDecision",
                    Decision = "Granted",
                    Created = regulatorTime,
                    DecisionDate = regulatorTime
                },
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegistrationApplicationSubmitted",
                    Created = producerTime,
                    Comments = "Late Producer Comment"
                }
            };

            // Act
            MergeCosmosUpdates(events, item);

            // Assert
            Assert.AreEqual(producerTime, item.ProducerCommentDate);
            Assert.AreEqual(regulatorTime, item.RegulatorDecisionDate);
            // ProducerCommentDate > RegulatorDecisionDate triggers Updated
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void ProducerAndRegulatorDecisions_ProducerEarlier_NoStatusUpdate()
        {
            // Arrange
            var regulatorTime = DateTime.UtcNow;
            var producerTime = DateTime.UtcNow.AddMinutes(-10);
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Granted);

            var events = new List<AbstractCosmosSubmissionEvent>
            {
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegulatorRegistrationDecision",
                    Decision = "Granted",
                    Created = regulatorTime,
                    DecisionDate = regulatorTime,
                    RegistrationReferenceNumber = "REF123"
                },
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegistrationApplicationSubmitted",
                    Created = producerTime,
                    Comments = "Earlier Producer Comment"
                }
            };

            // Act
            MergeCosmosUpdates(events, item);

            // Assert
            Assert.AreEqual(producerTime, item.ProducerCommentDate);
            Assert.AreEqual(regulatorTime, item.RegulatorDecisionDate);
            // ProducerCommentDate NOT > RegulatorDecisionDate, no update
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Granted, item.SubmissionDetails.Status);
        }

        [TestMethod]
        public void ProducerAndRegulatorDecisions_EitherIsNull_NoFinalUpdateCheck()
        {
            // Arrange
            var regulatorTime = DateTime.UtcNow;
            var item = CreateDefaultItem("APP123", RegistrationSubmissionStatus.Pending);

            var events = new List<AbstractCosmosSubmissionEvent>
            {
                // Regulator decision sets RegulatorDecisionDate
                new() {
                    AppReferenceNumber = "APP123",
                    Type = "RegulatorRegistrationDecision",
                    Decision = "Refused",
                    Created = regulatorTime,
                    DecisionDate = regulatorTime
                }
                // No producer event here
            };

            // Act
            MergeCosmosUpdates(events, item);

            // Assert
            Assert.IsNotNull(item.RegulatorDecisionDate);
            Assert.IsNull(item.ProducerCommentDate);
            // Because ProducerCommentDate is null, final condition not triggered
            Assert.AreEqual(RegistrationSubmissionStatus.Refused, item.SubmissionStatus);
            Assert.AreEqual(RegistrationSubmissionStatus.Refused, item.SubmissionDetails.Status);
        }

        // Helper: Create a default item with minimal setup
        private RegistrationSubmissionOrganisationDetailsResponse CreateDefaultItem(string appRef, RegistrationSubmissionStatus initialStatus)
        {
            return new RegistrationSubmissionOrganisationDetailsResponse
            {
                ApplicationReferenceNumber = appRef,
                SubmissionStatus = initialStatus,
                SubmissionDetails = new RegistrationSubmissionOrganisationSubmissionSummaryDetails
                {
                    Status = initialStatus
                }
            };
        }
    }
}
