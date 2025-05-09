using AutoFixture;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.Services.ReprocessorExporter.Registrations;

[TestClass]
public class RegistrationServiceTests
{
    private Mock<IRegistrationServiceClient> _mockRegistrationServiceClient = null!;
    private Mock<IAccountServiceClient> _mockAccountsServiceClient = null!;
    private Mock<IPaymentServiceClient> _mockPaymentServiceClient = null!;
    private RegistrationService _service = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockRegistrationServiceClient = new Mock<IRegistrationServiceClient>();
        _mockAccountsServiceClient = new Mock<IAccountServiceClient>();
        _mockPaymentServiceClient = new Mock<IPaymentServiceClient>();
        _service = new RegistrationService(_mockRegistrationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateRegulatorApplicationTaskDto>();
        _mockRegistrationServiceClient.Setup(client => client.UpdateRegulatorApplicationTaskStatus(requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateRegulatorApplicationTaskStatus(requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task UpdateRegulatorRegistrationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateRegulatorRegistrationTaskDto>();
        _mockRegistrationServiceClient.Setup(client => client.UpdateRegulatorRegistrationTaskStatus(requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateRegulatorRegistrationTaskStatus(requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetRegistrationByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationOverviewDto>();
        _mockRegistrationServiceClient.Setup(client => client.GetRegistrationByRegistrationId(1))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetRegistrationByRegistrationId(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetRegistrationMaterialByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationMaterialDetailsDto>();
        _mockRegistrationServiceClient.Setup(client => client.GetRegistrationMaterialByRegistrationMaterialId(1))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetRegistrationMaterialByRegistrationMaterialId(1);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateMaterialOutcomeRequestDto>();
        _mockRegistrationServiceClient.Setup(client => client.UpdateMaterialOutcomeByRegistrationMaterialId(1, requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateMaterialOutcomeByRegistrationMaterialId(1, requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetWasteLicenceByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = 1;
        var expectedDto = _fixture.Create<RegistrationMaterialWasteLicencesDto>();
        _mockRegistrationServiceClient.Setup(client => client.GetWasteLicenceByRegistrationMaterialId(id))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetWasteLicenceByRegistrationMaterialId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetReprocessingIOByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = 1;
        var expectedDto = _fixture.Create<RegistrationMaterialReprocessingIODto>();
        _mockRegistrationServiceClient.Setup(client => client.GetReprocessingIOByRegistrationMaterialId(id))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetReprocessingIOByRegistrationMaterialId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldReturnMappedDto()
    {
        // Arrange
        var registrationId = 1;
        var registrationSiteAddress = _fixture.Create<RegistrationSiteAddressDto>();
        var nationName = "England";

        _mockRegistrationServiceClient
            .Setup(client => client.GetSiteAddressByRegistrationId(registrationId))
            .ReturnsAsync(registrationSiteAddress);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationNameById(registrationSiteAddress.NationId))
            .ReturnsAsync(nationName);

        _service = new RegistrationService(_mockRegistrationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        var result = await _service.GetSiteAddressByRegistrationId(registrationId);

        // Assert
        result.Should().NotBeNull();
        result.SiteAddress.Should().BeEquivalentTo(registrationSiteAddress.SiteAddress);
        result.GridReference.Should().Be(registrationSiteAddress.GridReference);
        result.LegalCorrespondenceAddress.Should().BeEquivalentTo(registrationSiteAddress.LegalCorrespondenceAddress);
        result.NationName.Should().Be(nationName);
    }

    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldCallClientsExactlyOnce()
    {
        // Arrange
        var registrationId = 2;
        var registrationSiteAddress = _fixture.Create<RegistrationSiteAddressDto>();
        var nationName = "Wales";

        _mockRegistrationServiceClient
            .Setup(client => client.GetSiteAddressByRegistrationId(registrationId))
            .ReturnsAsync(registrationSiteAddress);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationNameById(registrationSiteAddress.NationId))
            .ReturnsAsync(nationName);

        _service = new RegistrationService(_mockRegistrationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        await _service.GetSiteAddressByRegistrationId(registrationId);

        // Assert
        _mockRegistrationServiceClient.Verify(c => c.GetSiteAddressByRegistrationId(registrationId), Times.Once);
        _mockAccountsServiceClient.Verify(c => c.GetNationNameById(registrationSiteAddress.NationId), Times.Once);
    }


    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldReturnExpectedDto()
    {
        // Arrange
        var registrationId = 10;
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();
        _mockRegistrationServiceClient.Setup(client => client.GetAuthorisedMaterialByRegistrationId(registrationId))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetAuthorisedMaterialByRegistrationId(registrationId);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldCallClientExactlyOnce()
    {
        // Arrange
        var registrationId = 42;
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();
        _mockRegistrationServiceClient.Setup(client => client.GetAuthorisedMaterialByRegistrationId(registrationId))
                   .ReturnsAsync(expectedDto);

        // Act
        await _service.GetAuthorisedMaterialByRegistrationId(registrationId);

        // Assert
        _mockRegistrationServiceClient.Verify(client => client.GetAuthorisedMaterialByRegistrationId(registrationId), Times.Once);
    }

    [TestMethod]
    public async Task GetSamplingPlanByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = 1;
        var expectedDto = _fixture.Create<RegistrationMaterialSamplingPlanDto>();
        _mockRegistrationServiceClient.Setup(client => client.GetSamplingPlanByRegistrationMaterialId(id))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetSamplingPlanByRegistrationMaterialId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByRegistrationMaterialId_ShouldReturnMappedDto()
    {
        // Arrange
        var id = 1;
        var registrationFeeRequestInfo = _fixture.Create<RegistrationFeeContextDto>();
        var organisationName = "Test Org";
        var nationName = "Scotland";
        var paymentFee = 150.75m;

        _mockRegistrationServiceClient
            .Setup(client => client.GetRegistrationFeeRequestByRegistrationMaterialId(id))
            .ReturnsAsync(registrationFeeRequestInfo);

        _mockAccountsServiceClient
            .Setup(client => client.GetOrganisationNameById(id))
            .ReturnsAsync(organisationName);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationNameById(registrationFeeRequestInfo.NationId))
            .ReturnsAsync(nationName);

        _mockPaymentServiceClient
            .Setup(client => client.GetRegistrationPaymentFee(
                registrationFeeRequestInfo.MaterialName,
                nationName,
                registrationFeeRequestInfo.CreatedDate,
                registrationFeeRequestInfo.ApplicationType.ToString(),
                registrationFeeRequestInfo.Reference))
            .ReturnsAsync(paymentFee);

        _service = new RegistrationService(_mockRegistrationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        var result = await _service.GetPaymentFeeDetailsByRegistrationMaterialId(id);

        // Assert
        result.Should().NotBeNull();
        result.RegistrationMaterialId.Should().Be(id);
        result.OrganisationName.Should().Be(organisationName);
        result.SiteAddress.Should().BeEquivalentTo(registrationFeeRequestInfo.SiteAddress);
        result.ReferenceNumber.Should().Be(registrationFeeRequestInfo.Reference);
        result.MaterialName.Should().Be(registrationFeeRequestInfo.MaterialName);
        result.ApplicationType.Should().Be(registrationFeeRequestInfo.ApplicationType);
        result.SubmittedDate.Should().Be(registrationFeeRequestInfo.CreatedDate);
        result.FeeAmount.Should().Be(paymentFee);
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByRegistrationMaterialId_ShouldCallClientsExactlyOnce()
    {
        // Arrange
        var id = 2;
        var registrationFeeRequestInfo = _fixture.Create<RegistrationFeeContextDto>();
        var organisationName = "Org Name";
        var nationName = "Northern Ireland";
        var paymentFee = 200.00m;

        _mockRegistrationServiceClient
            .Setup(client => client.GetRegistrationFeeRequestByRegistrationMaterialId(id))
            .ReturnsAsync(registrationFeeRequestInfo);

        _mockAccountsServiceClient
            .Setup(client => client.GetOrganisationNameById(id))
            .ReturnsAsync(organisationName);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationNameById(registrationFeeRequestInfo.NationId))
            .ReturnsAsync(nationName);

        _mockPaymentServiceClient
            .Setup(client => client.GetRegistrationPaymentFee(
                registrationFeeRequestInfo.MaterialName,
                nationName,
                registrationFeeRequestInfo.CreatedDate,
                registrationFeeRequestInfo.ApplicationType.ToString(),
                registrationFeeRequestInfo.Reference))
            .ReturnsAsync(paymentFee);

        _service = new RegistrationService(_mockRegistrationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        await _service.GetPaymentFeeDetailsByRegistrationMaterialId(id);

        // Assert
        _mockRegistrationServiceClient.Verify(c => c.GetRegistrationFeeRequestByRegistrationMaterialId(id), Times.Once);
        _mockAccountsServiceClient.Verify(c => c.GetOrganisationNameById(id), Times.Once);
        _mockAccountsServiceClient.Verify(c => c.GetNationNameById(registrationFeeRequestInfo.NationId), Times.Once);
        _mockPaymentServiceClient.Verify(c => c.GetRegistrationPaymentFee(
            registrationFeeRequestInfo.MaterialName,
            nationName,
            registrationFeeRequestInfo.CreatedDate,
            registrationFeeRequestInfo.ApplicationType.ToString(),
            registrationFeeRequestInfo.Reference), Times.Once);
    }

}