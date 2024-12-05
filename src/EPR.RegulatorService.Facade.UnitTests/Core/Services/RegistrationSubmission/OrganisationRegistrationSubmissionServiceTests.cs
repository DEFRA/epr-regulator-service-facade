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

        // Act 
        var result = _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Producer, orgId, year);

        // Assert  
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length == 15);
        Assert.IsTrue(result.StartsWith('R'));
        Assert.IsTrue(result.StartsWith($"R{year}"));
        Assert.IsTrue(result.StartsWith($"R{year}EP"));
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


        List<AbstractCosmosSubmissionEvent>  updateDeltaRegistrationDecisions = deltaRegistrationDecisions.Select(x => new AbstractCosmosSubmissionEvent
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
    public async Task Should_return_valid_ReferenceNumber_with_null_year()
    {
        //Arrange  
        string year = (DateTime.Now.Year % 100).ToString("D2");
        string orgId = "123456";
        
        // Act 
        var result = _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Producer, orgId, null);

        // Assert  
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Length == 15);
        Assert.IsTrue(result.StartsWith('R'));
        Assert.IsTrue(result.StartsWith($"R{year}"));
        Assert.IsTrue(result.StartsWith($"R{year}EP"));
    }

    [TestMethod]
    public async Task Should_return_valid_ReferenceNumber_For_Exporter_With_Steel()
    {
        //Arrange  
        string year = (DateTime.Now.Year % 100).ToString("D2");
        string orgId = "123456";
        
        // Act 
        var result = _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Exporter, orgId, null, MaterialType.Steel);

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
        
        // Act 
        var result = _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Reprocessor, orgId, null, MaterialType.Plastic);

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
        Assert.ThrowsException<ArgumentNullException>(() => _sut.GenerateReferenceNumber(CountryName.Eng, RegistrationSubmissionType.Producer, string.Empty, null) );
    }
} 
