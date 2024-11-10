﻿using EPR.RegulatorService.Facade.Core.Enums;
using EPR.RegulatorService.Facade.Core.Services.CommonData;
using EPR.RegulatorService.Facade.Core.Services.RegistrationSubmission;
using EPR.RegulatorService.Facade.Core.Services.Submissions;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.RegistrationSubmission;

[TestClass]
public class RegistrationSubmissionServiceTests
{
    private readonly Mock<ISubmissionService> _submissionsServiceMock = new();
    private readonly Mock<ICommonDataService> _commonDataServiceMock = new();
    private OrganisationRegistrationSubmissionService _sut;

    [TestInitialize]
    public void Setup()
    {
        _sut = new OrganisationRegistrationSubmissionService(_commonDataServiceMock.Object, 
                                                             _submissionsServiceMock.Object);
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
