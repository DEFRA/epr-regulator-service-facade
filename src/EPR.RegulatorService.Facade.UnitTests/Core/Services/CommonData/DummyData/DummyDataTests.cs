using EPR.RegulatorService.Facade.Core.Models.Applications;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Models.Responses.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData;
using EPR.RegulatorService.Facade.UnitTests.TestHelpers;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using static EPR.RegulatorService.Facade.Core.Services.CommonData.DummyData.JsonOrganisationRegistrationHandler;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.CommonData.DummyData;

/// <summary>
/// These classes merely test the functionality of loading dummy data into the API and out to the client
/// The models are approximately correct, and may be expanded or contracted, depending on the data architecture
/// they rely on to be decided.
/// </summary>
[TestClass]
public class DummyDataTests
{
    private Guid _userId = Guid.NewGuid();
    private readonly string testDataFilePath = ".\\Core\\Services\\CommonData\\DummyData\\dummydata.json";

    OrganisationRegistrationDataCollection? _testData = null;

    public TestContext TestContext
    {
        get; set;
    }

    [TestInitialize]
    public void Setup()
    {
        _testData = TestRunDataHelper.LoadDataFile<OrganisationRegistrationDataCollection>(testDataFilePath, TestContext);
    }

    [TestMethod]
    public void OrganisationDummDataLoader_LoadsDummyDataFromFile()
    {
        IDummyDataLoader<OrganisationRegistrationDataCollection> dataLoader = new OrganisationDummyDataLoader();

        var expectedData = _testData;

        var orgDataCollection = dataLoader.LoadData(testDataFilePath);
        Assert.IsNotNull(orgDataCollection);
        Assert.AreEqual(expectedData.Items.Count, orgDataCollection.Items.Count);
    }

    [TestCleanup]
    public void Cleanup()
    {
        OrganisationRegistrationDummyDataCache.Reset();
    }

    [TestMethod]
    public void DataCache_LoadsDataWhenTheValueIsRequested()
    {
        var expectedData = _testData;

        Mock<OrganisationDummyDataLoader> mockLoader = new Mock<OrganisationDummyDataLoader>();
        mockLoader.Setup(m=> m.LoadData(testDataFilePath)).Returns(expectedData);

        var cacheRequest = OrganisationRegistrationDummyDataCache.GetOrAdd(testDataFilePath, mockLoader.Object.LoadData).Value;
        var newCacheRequest = OrganisationRegistrationDummyDataCache.GetOrAdd(testDataFilePath, mockLoader.Object.LoadData).Value;

        Assert.AreEqual(cacheRequest.Items.Count, newCacheRequest.Items.Count);
        mockLoader.Verify(m => m.LoadData(testDataFilePath), Times.Exactly(1));
    }

    [TestMethod]
    public void DataCache_DoesntLoadsDataWhenTheValueIsNotRequired()
    {
        var expectedData = _testData;

        var mockLoader = new Mock<OrganisationDummyDataLoader>();
        mockLoader.Setup(m => m.LoadData(testDataFilePath)).Returns(expectedData);

        var cacheRequest = OrganisationRegistrationDummyDataCache.GetOrAdd(testDataFilePath, mockLoader.Object.LoadData);

        Assert.IsFalse(cacheRequest.IsValueCreated);
        mockLoader.Verify(m => m.LoadData(testDataFilePath), Times.Exactly(0));
    }

    [TestMethod]
    public async Task FilterRegistrations_Will_Not_Filter_Data_When_No_Filter_Params_Supplied()
    {
        var testData = _testData;

        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId
        };

        var resultData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request);
        Assert.AreEqual(resultData.Count, testData.Items.Count);
    }

    [TestMethod]
    public async Task FilterRegistrations_Will_Filter_OrganisationName()
    {
        var testData = _testData;
        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            OrganisationName = "Wuckert LLC"
        };

        var resultData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request);
        Assert.IsTrue(resultData.Count == 1);
    }

    [TestMethod]
    public async Task FilterRegistrations_Will_Filter_OrganisationType()
    {
        var testData = _testData;
        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            OrganisationType = "small"
        };

        var resultData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request);
        Assert.IsTrue(resultData.Count > 1);
    }

    [TestMethod]
    public async Task FilterRegistrations_Will_Filter_OrganisationReference()
    {
        var testData = _testData;
        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            OrganisationReference = "BIAAGCIMRMCFU"
        };

        var resultData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request);
        Assert.IsTrue(resultData.Count == 1);
        Assert.IsTrue(resultData[0].OrganisationName == "Wehner Group");

        request.OrganisationReference = "CQO4F63QKS4ZUESX5PJTRG2DZO";
        resultData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request);
        Assert.IsTrue(resultData.Count == 1);
        Assert.IsTrue(resultData[0].OrganisationName == "Mosciski - Rempel");
    }

    [TestMethod]
    public async Task FilterRegistrations_Will_Filter_OnMultipleClauses()
    {
        var testData = _testData;
        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            OrganisationType = "large",
            Statuses = "granted refused"
        };

        var resultData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request);
        resultData.Exists(d => !request.Statuses.Contains(d.RegistrationStatus)).Should().BeFalse();
        resultData.Exists(d => !request.OrganisationType.Equals(d.OrganisationType)).Should().BeFalse();
    }

    [TestMethod]
    public async Task FilterRegistrations_Will_Filter_OnRegistrationYears()
    {
        var testData = _testData;
        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            OrganisationType = "large",
            Statuses = "granted pending",
            RegistrationYears = "2023"
        };

        var resultData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request);
        Assert.IsTrue(resultData.Count > 1);
    }

    [TestMethod]
    public async Task Filter_Will_Remove_All_But_Small_And_Pagination_Will_Return_3_Items()
    {
        var testData = _testData;

        var request = new GetOrganisationRegistrationRequest
        {
            OrganisationType = "small",
            PageNumber = 1,
            PageSize = 3
        };

        var convertedData = JsonOrganisationRegistrationHandler.FilterRegistrations(testData, request).Select(data => (OrganisationRegistrationSummaryResponse)data).ToList();
        var result = JsonOrganisationRegistrationHandler.Paginate(convertedData, request.PageNumber.Value, request.PageSize.Value);
        var objRet = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSummaryResponse>>(result);

        objRet.Items.Exists(x => !x.OrganisationType.Equals(EPR.RegulatorService.Facade.Core.Enums.OrganisationType.small)).Should().BeFalse();
        Assert.IsTrue(objRet.Items.Count == 3);
    }

    [TestMethod]
    public async Task JsonOrganisationRegistrationHandler_Will_Return_StatusOK_When_Successful()
    {
        var expectedData = _testData;
        var mockLoader = new Mock<IDummyDataLoader<OrganisationRegistrationDataCollection>>();
        mockLoader.Setup(m => m.LoadData(testDataFilePath)).Returns(expectedData);
        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            PageNumber = 1,
            PageSize = 20
        };

        var subject = new JsonOrganisationRegistrationHandler(testDataFilePath, mockLoader.Object);
        var result = await subject.GetOrganisationRegistrations(request);

        Assert.IsTrue(result.IsSuccessStatusCode);
    }

    [TestMethod]
    public async Task JsonOrganisationRegistrationHandler_Will_Load_Data_As_Expected()
    {
        var testData = _testData;
        var mockLoader = new Mock<IDummyDataLoader<OrganisationRegistrationDataCollection>>();
        mockLoader.Setup(m => m.LoadData(testDataFilePath)).Returns(testData);

        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            PageNumber = 1,
            PageSize = 20,
            RegistrationYears = "2024"
        };

        var subject = new JsonOrganisationRegistrationHandler(testDataFilePath, mockLoader.Object);
        var result = await subject.GetOrganisationRegistrations(request);

        Assert.IsTrue(result.IsSuccessStatusCode);

        var returnedJson = await result.Content.ReadAsStringAsync();
        var returnedData = JsonSerializer.Deserialize<PaginatedResponse<OrganisationRegistrationSummaryResponse>>(returnedJson);
        Assert.IsNotNull(returnedData);
        Assert.IsTrue(returnedData.Items.Count >= 1);
    }

    [TestMethod]
    public async Task JsonOrganisationRegistrationHandler_Will_Return_Failure_When_No_Data()
    {
        var mockLoader = new Mock<IDummyDataLoader<OrganisationRegistrationDataCollection>>();
        mockLoader.Setup(m => m.LoadData(testDataFilePath)).Throws<FileNotFoundException>();

        var request = new GetOrganisationRegistrationRequest
        {
            UserId = _userId,
            PageNumber = 1,
            PageSize = 20,
            RegistrationYears = "2024"
        };

        var subject = new JsonOrganisationRegistrationHandler(testDataFilePath, mockLoader.Object);
        var result = await subject.GetOrganisationRegistrations(request);
        Assert.IsTrue(result.StatusCode == HttpStatusCode.InternalServerError);

    }
}

