using EPR;
using EPR.RegulatorService;
using EPR.RegulatorService.Facade;
using EPR.RegulatorService.Facade.Core.Extensions;
using EPR.RegulatorService.Facade.Core.Models.Requests.RegistrationSubmissions;
using EPR.RegulatorService.Facade.UnitTests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web;

namespace EPR.RegulatorService.Facade.UnitTests.Extensions;

[TestClass()]
public class GetOrganisationRegistrationSubmissionsFilterExtensionsTests
{
    [TestMethod]
    public void GenerateQueryString_AllParametersPresent_ReturnsCorrectQueryString()
    {
        // Arrange
        var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter
        {
            OrganisationName = "Org1 Org2",
            OrganisationReference = "123 456",
            RelevantYears = "2023 2024",
            Statuses = "Pending Accepted",
            OrganisationType = "DirectProducer ComplianceScheme",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var queryString = filter.GenerateQueryString();

        // Assert
        var expected = HttpUtility.UrlEncode("OrganisationNameCommaSeparated") + "=" + HttpUtility.UrlEncode("Org1,Org2") + "&" +
                       HttpUtility.UrlEncode("OrganisationIDCommaSeparated") + "=" + HttpUtility.UrlEncode("123,456") + "&" +
                       HttpUtility.UrlEncode("RelevantYearCommaSeparated") + "=" + HttpUtility.UrlEncode("2023,2024") + "&" +
                       HttpUtility.UrlEncode("SubmissionStatusCommaSeparated") + "=" + HttpUtility.UrlEncode("Pending,Accepted") + "&" +
                       HttpUtility.UrlEncode("OrganisationTypesCommaSeparated") + "=" + HttpUtility.UrlEncode("DirectProducer,ComplianceScheme") + "&" +
                       HttpUtility.UrlEncode("PageNumber") + "=" + HttpUtility.UrlEncode("1") + "&" +
                       HttpUtility.UrlEncode("PageSize") + "=" + HttpUtility.UrlEncode("10");

        Assert.AreEqual(expected, queryString);
    }

    [TestMethod]
    public void GenerateQueryString_NullOrEmptyParameters_ExcludesNullValues()
    {
        // Arrange
        var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter
        {
            OrganisationName = null,
            OrganisationReference = string.Empty,
            RelevantYears = "2023",
            Statuses = "",
            OrganisationType = "DirectProducer",
            PageNumber = null,
            PageSize = 10
        };

        // Act
        var queryString = filter.GenerateQueryString();

        // Assert
        var expected = "RelevantYearCommaSeparated=2023" +
                       "&OrganisationTypesCommaSeparated=DirectProducer" +
                       "&PageSize=10";

        Assert.AreEqual(expected, queryString);
    }

    [TestMethod]
    public void GenerateQueryString_SpacesInParameters_AreConvertedToCommas()
    {
        // Arrange
        var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter
        {
            OrganisationName = "Org1  Org2    Org3",
            OrganisationReference = " 123   456 ",
            RelevantYears = "   ",
            Statuses = "Pending Accepted Queried",
            OrganisationType = null,
            PageNumber = 2,
            PageSize = null
        };

        // Act
        var queryString = filter.GenerateQueryString();

        // Assert
        var expected = HttpUtility.UrlEncode("OrganisationNameCommaSeparated") + "=" + HttpUtility.UrlEncode("Org1,Org2,Org3") + "&" +
                       HttpUtility.UrlEncode("OrganisationIDCommaSeparated") + "=" + HttpUtility.UrlEncode("123,456") + "&" +
                       HttpUtility.UrlEncode("SubmissionStatusCommaSeparated") + "=" + HttpUtility.UrlEncode("Pending,Accepted,Queried") + "&" +
                       HttpUtility.UrlEncode("PageNumber") + "=" + HttpUtility.UrlEncode("2");

        Assert.AreEqual(expected, queryString);
    }

    [TestMethod]
    public void GenerateQueryString_SpecialCharacters_AreUrlEncoded()
    {
        // Arrange
        var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter
        {
            OrganisationName = "Org&1 Org=2",
            OrganisationReference = "123+456",
            RelevantYears = "2023,2024",
            Statuses = "Pending/Accepted",
            OrganisationType = "Direct Producer",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var queryString = filter.GenerateQueryString();

        // Assert
        var sdexpected = "OrganisationNameCommaSeparated=Org%261,Org%3D2" +
                       "&OrganisationIDCommaSeparated=123%2B456" +
                       "&RelevantYearCommaSeparated=2023%2C2024" +
                       "&SubmissionStatusCommaSeparated=Pending%2FAccepted" +
                       "&OrganisationTypesCommaSeparated=Direct%20Producer" +
                       "&PageNumber=1" +
                       "&PageSize=10";

        var expected = HttpUtility.UrlEncode("OrganisationNameCommaSeparated") + "=" + HttpUtility.UrlEncode("Org&1,Org=2") + "&" +
               HttpUtility.UrlEncode("OrganisationIDCommaSeparated") + "=" + HttpUtility.UrlEncode("123+456") + "&" +
               HttpUtility.UrlEncode("RelevantYearCommaSeparated") + "=" + HttpUtility.UrlEncode("2023,2024") + "&" +
               HttpUtility.UrlEncode("SubmissionStatusCommaSeparated") + "=" + HttpUtility.UrlEncode("Pending/Accepted") + "&" +
               HttpUtility.UrlEncode("OrganisationTypesCommaSeparated") + "=" + HttpUtility.UrlEncode("Direct,Producer") + "&" +
               HttpUtility.UrlEncode("PageNumber") + "=" + HttpUtility.UrlEncode("1") + "&" +
               HttpUtility.UrlEncode("PageSize") + "=" + HttpUtility.UrlEncode("10");
        Assert.AreEqual(expected, queryString);
    }

    [TestMethod]
    public void GenerateQueryString_AllNullParameters_ReturnsEmptyQueryString()
    {
        // Arrange
        var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter
        {
            OrganisationName = null,
            OrganisationReference = null,
            RelevantYears = null,
            Statuses = null,
            OrganisationType = null,
            PageNumber = null,
            PageSize = null
        };

        // Act
        var queryString = filter.GenerateQueryString();

        // Assert
        Assert.AreEqual(string.Empty, queryString);
    }

    [TestMethod]
    public void GenerateQueryString_EmptyFilter_ReturnsPagingInfoOnly()
    {
        // Arrange
        var expected = HttpUtility.UrlEncode("PageNumber") + "=" + HttpUtility.UrlEncode("1") + "&" +
                       HttpUtility.UrlEncode("PageSize") + "=" + HttpUtility.UrlEncode("20");

        var filter = new GetOrganisationRegistrationSubmissionsCommonDataFilter();

        // Act
        var queryString = filter.GenerateQueryString();

        // Assert
        Assert.AreEqual(expected, queryString);
    }

}