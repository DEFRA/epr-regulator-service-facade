using AutoFixture;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Accreditations;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Accreditations;
using FluentAssertions;
using Moq;

namespace EPR.RegulatorService.Facade.UnitTests.Services.ReprocessorExporter.Accreditations;

[TestClass]
public class AccreditationServiceTests
{
    private Mock<IAccreditationServiceClient> _mockAccreditationServiceClient = null!;
    private Mock<IAccountServiceClient> _mockAccountsServiceClient = null!;
    private Mock<IPaymentServiceClient> _mockPaymentServiceClient = null!;
    private AccreditationService _service = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockAccreditationServiceClient = new Mock<IAccreditationServiceClient>();
        _mockAccountsServiceClient = new Mock<IAccountServiceClient>();
        _mockPaymentServiceClient = new Mock<IPaymentServiceClient>();
        _service = new AccreditationService(_mockAccreditationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByAccreditationMaterialId_ShouldReturnMappedPaymentFeeDetailsDto_FromAccountAndPaymentServices()
    {
        // Arrange
        var id = 1;
        var accreditationFeeContextDto = _fixture.Create<AccreditationFeeContextDto>();
        var organisationName = "Green Ltd";

        var registeredMaterialId = Guid.NewGuid();

        var nationDetails = new NationDetailsResponseDto { Name = "England", NationCode = "GB-ENG" };

        var paymentFee = 3000.00m;

        _mockAccreditationServiceClient
            .Setup(client => client.GetAccreditationFeeRequestByRegistrationMaterialId(It.IsAny<Guid>()))
            .ReturnsAsync(accreditationFeeContextDto);

        _mockAccountsServiceClient
            .Setup(client => client.GetOrganisationNameById(accreditationFeeContextDto.OrganisationId))
            .ReturnsAsync(organisationName);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationDetailsById(accreditationFeeContextDto.NationId))
            .ReturnsAsync(nationDetails);

        _mockPaymentServiceClient
            .Setup(client => client.GetAccreditationPaymentFee(
                accreditationFeeContextDto.MaterialName,
                nationDetails.NationCode,
                accreditationFeeContextDto.CreatedDate,
                accreditationFeeContextDto.ApplicationType.ToString(),
                accreditationFeeContextDto.ApplicationReferenceNumber))
            .ReturnsAsync(paymentFee);

        _service = new AccreditationService(_mockAccreditationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        var result = await _service.GetPaymentFeeDetailsByAccreditationMaterialId(registeredMaterialId);

        // Assert
        result.Should().NotBeNull();
        result.RegistrationMaterialId.Should().Be(registeredMaterialId);
        result.OrganisationName.Should().Be(organisationName);
        result.SiteAddress.Should().BeEquivalentTo(accreditationFeeContextDto.SiteAddress);
        result.ApplicationReferenceNumber.Should().Be(accreditationFeeContextDto.ApplicationReferenceNumber);
        result.MaterialName.Should().Be(accreditationFeeContextDto.MaterialName);
        result.ApplicationType.Should().Be(accreditationFeeContextDto.ApplicationType);
        result.SubmittedDate.Should().Be(accreditationFeeContextDto.CreatedDate);
        result.FeeAmount.Should().Be(paymentFee);
    }
    
    [TestMethod]
    public async Task GetPaymentFeeDetailsByAccreditationMaterialId_ShouldThrowsException_WhenServiceFails()
    {

        var id = Guid.NewGuid();

        _mockAccreditationServiceClient
            .Setup(client => client.GetAccreditationFeeRequestByRegistrationMaterialId(id))
            .ThrowsAsync(new Exception("Service unavailable"));


        _service = new AccreditationService(_mockAccreditationServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        await FluentActions.Invoking(() => _service.GetPaymentFeeDetailsByAccreditationMaterialId(id))
                            .Should().ThrowAsync<Exception>()
                            .WithMessage("Service unavailable");

    }

    [TestMethod]
    public async Task MarkAsDulyMadeByRegistrationMaterialId_ShouldReturnTrue_WhenClientCallSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestDto = _fixture.Create<AccreditationMarkAsDulyMadeRequestDto>();

        var expectedDto = new AccreditationMarkAsDulyMadeRequestDto()
        {
            DulyMadeDate = requestDto.DulyMadeDate,
            DeterminationDate = requestDto.DeterminationDate

        };

        _mockAccreditationServiceClient
            .Setup(client => client.MarkAccreditationMaterialStatusAsDulyMade(It.Is<AccreditationMarkAsDulyMadeWithUserIdDto>(dto =>
                dto.DulyMadeDate == expectedDto.DulyMadeDate &&
                dto.DeterminationDate == expectedDto.DeterminationDate &&
                dto.DulyMadeBy == userId
            )))
            .ReturnsAsync(true);

        // Act
        var result = await _service.MarkAccreditationMaterialStatusAsDulyMade(userId, requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange

        var userId = Guid.NewGuid();

        var requestDto = _fixture.Create<UpdateAccreditationMaterialTaskStatusWithUserIdDto>();
        _mockAccreditationServiceClient.Setup(client => client.UpdateAccreditationMaterialTaskStatus(requestDto))
            .ReturnsAsync(true);


        _mockAccreditationServiceClient.Setup(client => client.UpdateAccreditationMaterialTaskStatus(It.Is<UpdateAccreditationMaterialTaskStatusWithUserIdDto>(dto => 
                        dto.AccreditationId== requestDto.AccreditationId &&
                        dto.RegistrationMaterialId== requestDto.RegistrationMaterialId &&
                        dto.TaskId== requestDto.TaskId &&
                        dto.TaskStatus== requestDto.TaskStatus &&
                        dto.Comments==requestDto.Comments &&    
                        dto.UpdatedByUserId == userId)))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAccreditationMaterialTaskStatus(userId, requestDto);

        // Assert
        result.Should().BeTrue();
    }
}