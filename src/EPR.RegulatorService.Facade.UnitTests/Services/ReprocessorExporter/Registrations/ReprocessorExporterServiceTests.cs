using AutoFixture;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter;
using EPR.RegulatorService.Facade.Core.Clients.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Constants;
using EPR.RegulatorService.Facade.Core.Models.ReprocessorExporter.Registrations;
using EPR.RegulatorService.Facade.Core.Services.ReprocessorExporter.Registrations;
using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.Services.ReprocessorExporter.Registrations;

[TestClass]
public class ReprocessorExporterServiceTests
{
    private Mock<IReprocessorExporterServiceClient> _mockReprocessorExporterServiceClient = null!;
    private Mock<IAccountServiceClient> _mockAccountsServiceClient = null!;
    private Mock<IPaymentServiceClient> _mockPaymentServiceClient = null!;
    private ReprocessorExporterService _service = null!;
    private Fixture _fixture = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockReprocessorExporterServiceClient = new Mock<IReprocessorExporterServiceClient>();
        _mockAccountsServiceClient = new Mock<IAccountServiceClient>();
        _mockPaymentServiceClient = new Mock<IPaymentServiceClient>();
        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);
        _fixture = new Fixture();
    }

    [TestMethod]
    public async Task UpdateRegulatorApplicationTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateRegulatorApplicationTaskDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.UpdateRegulatorApplicationTaskStatus(requestDto))
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
        _mockReprocessorExporterServiceClient.Setup(client => client.UpdateRegulatorRegistrationTaskStatus(requestDto))
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
        _mockReprocessorExporterServiceClient.Setup(client => client.GetRegistrationByRegistrationId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175")))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetRegistrationByRegistrationId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetRegistrationMaterialByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var expectedDto = _fixture.Create<RegistrationMaterialDetailsDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetRegistrationMaterialByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175")))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetRegistrationMaterialByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"));

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task UpdateMaterialOutcomeByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<UpdateMaterialOutcomeRequestDto>();

        var referenceDto = new RegistrationAccreditationReferenceDto
        {
            RegistrationType = "R",
            RelevantYear = "25",
            NationId = 100,
            ApplicationType = "Reprocessor",
            OrgCode = "ORG123",
            RandomDigits = "4567",
            MaterialCode = "PL"
        };

        var nationDetails = new NationDetailsResponseDto
        {
            Name = "England",
            NationCode = "GB-ENG"
        };

        var expectedReference = "R25ERORG1234567PL";

        var expectedDto = new UpdateMaterialOutcomeWithReferenceDto
        {
            Comments = requestDto.Comments,
            Status = requestDto.Status,
            RegistrationReferenceNumber = expectedReference
        };

        _mockReprocessorExporterServiceClient.Setup(client => client.GetRegistrationAccreditationReference(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175")))
            .ReturnsAsync(referenceDto);

        _mockAccountsServiceClient.Setup(client => client.GetNationDetailsById(referenceDto.NationId))
            .ReturnsAsync(nationDetails);

        _mockReprocessorExporterServiceClient.Setup(client =>
            client.UpdateMaterialOutcomeByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), It.Is<UpdateMaterialOutcomeWithReferenceDto>(
                x => x.Comments == expectedDto.Comments &&
                     x.Status == expectedDto.Status &&
                     x.RegistrationReferenceNumber == expectedDto.RegistrationReferenceNumber)))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateMaterialOutcomeByRegistrationMaterialId(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetWasteLicenceByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<RegistrationMaterialWasteLicencesDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetWasteLicenceByRegistrationMaterialId(id))
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
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<RegistrationMaterialReprocessingIODto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetReprocessingIOByRegistrationMaterialId(id))
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
        var registrationId = Guid.NewGuid();
        var registrationSiteAddress = _fixture.Create<RegistrationSiteAddressDto>();
        var nationDetails = new NationDetailsResponseDto { Name = "England", NationCode = "GB-ENG" };

        _mockReprocessorExporterServiceClient
            .Setup(client => client.GetSiteAddressByRegistrationId(registrationId))
            .ReturnsAsync(registrationSiteAddress);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationDetailsById(registrationSiteAddress.NationId))
            .ReturnsAsync(nationDetails);

        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        var result = await _service.GetSiteAddressByRegistrationId(registrationId);

        // Assert
        result.Should().NotBeNull();
        result.SiteAddress.Should().BeEquivalentTo(registrationSiteAddress.SiteAddress);
        result.GridReference.Should().Be(registrationSiteAddress.GridReference);
        result.LegalCorrespondenceAddress.Should().BeEquivalentTo(registrationSiteAddress.LegalCorrespondenceAddress);
        result.NationName.Should().Be(nationDetails.Name);
    }

    [TestMethod]
    public async Task GetSiteAddressByRegistrationId_ShouldCallClientsExactlyOnce()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var registrationSiteAddress = _fixture.Create<RegistrationSiteAddressDto>();
        var nationDetails = new NationDetailsResponseDto { Name = "Wales", NationCode = "GB-WLS" };

        _mockReprocessorExporterServiceClient
            .Setup(client => client.GetSiteAddressByRegistrationId(registrationId))
            .ReturnsAsync(registrationSiteAddress);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationDetailsById(registrationSiteAddress.NationId))
            .ReturnsAsync(nationDetails);

        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        await _service.GetSiteAddressByRegistrationId(registrationId);

        // Assert
        _mockReprocessorExporterServiceClient.Verify(c => c.GetSiteAddressByRegistrationId(registrationId), Times.Once);
        _mockAccountsServiceClient.Verify(c => c.GetNationDetailsById(registrationSiteAddress.NationId), Times.Once);
    }


    [TestMethod]
    public async Task GetAuthorisedMaterialByRegistrationId_ShouldReturnExpectedDto()
    {
        // Arrange
        var registrationId = Guid.NewGuid();
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetAuthorisedMaterialByRegistrationId(registrationId))
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
        var registrationId = Guid.NewGuid();
        var expectedDto = _fixture.Create<MaterialsAuthorisedOnSiteDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetAuthorisedMaterialByRegistrationId(registrationId))
                   .ReturnsAsync(expectedDto);

        // Act
        await _service.GetAuthorisedMaterialByRegistrationId(registrationId);

        // Assert
        _mockReprocessorExporterServiceClient.Verify(client => client.GetAuthorisedMaterialByRegistrationId(registrationId), Times.Once);
    }

    [TestMethod]
    public async Task GetSamplingPlanByRegistrationMaterialId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<RegistrationMaterialSamplingPlanDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetSamplingPlanByRegistrationMaterialId(id))
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
        var id = Guid.NewGuid();
        var registrationFeeRequestInfo = _fixture.Create<RegistrationFeeContextDto>();
        var organisationName = "Test Org";
        var nationDetails = new NationDetailsResponseDto { Name = "Scotland", NationCode = "GB-SCT" };

        _mockReprocessorExporterServiceClient
            .Setup(client => client.GetRegistrationFeeRequestByRegistrationMaterialId(id))
            .ReturnsAsync(registrationFeeRequestInfo);

        _mockAccountsServiceClient
            .Setup(client => client.GetOrganisationNameById(registrationFeeRequestInfo.OrganisationId))
            .ReturnsAsync(organisationName);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationDetailsById(registrationFeeRequestInfo.NationId))
            .ReturnsAsync(nationDetails);

        var paymentFeeResponse = _fixture.Create<PaymentFeeResponseDto>();

        _mockPaymentServiceClient
             .Setup(client => client.GetRegistrationPaymentFee(It.IsAny<PaymentFeeRequestDto>()))
             .ReturnsAsync(paymentFeeResponse);

        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        var result = await _service.GetPaymentFeeDetailsByRegistrationMaterialId(id);

        // Assert
        result.Should().NotBeNull();
        result.RegistrationMaterialId.Should().Be(id);
        result.OrganisationName.Should().Be(organisationName);
        result.SiteAddress.Should().BeEquivalentTo(registrationFeeRequestInfo.SiteAddress);
        result.ApplicationReferenceNumber.Should().Be(registrationFeeRequestInfo.ApplicationReferenceNumber);
        result.MaterialName.Should().Be(registrationFeeRequestInfo.MaterialName);
        result.ApplicationType.Should().Be(registrationFeeRequestInfo.ApplicationType);
        result.SubmittedDate.Should().Be(registrationFeeRequestInfo.CreatedDate);
        result.FeeAmount.Should().Be(paymentFeeResponse.RegistrationFee);
        result.Regulator.Should().Be(nationDetails.NationCode);
        result.PaymentMethod.Should().Be(paymentFeeResponse.PreviousPaymentDetail?.PaymentMethod);
        result.PaymentDate.Should().Be(paymentFeeResponse.PreviousPaymentDetail?.PaymentDate);
        result.DulyMadeDate.Should().Be(registrationFeeRequestInfo.DulyMadeDate);
        result.DeterminationDate.Should().Be(registrationFeeRequestInfo.DeterminationDate);
    }

    [TestMethod]
    public async Task GetPaymentFeeDetailsByRegistrationMaterialId_ShouldCallClientsExactlyOnce()
    {
        // Arrange
        var id = Guid.NewGuid();
        var registrationFeeRequestInfo = _fixture.Create<RegistrationFeeContextDto>();
        var organisationName = "Org Name";
        var nationDetails = new NationDetailsResponseDto { Name = "Northern Ireland", NationCode = "GB-NIR" };

        _mockReprocessorExporterServiceClient
            .Setup(client => client.GetRegistrationFeeRequestByRegistrationMaterialId(id))
            .ReturnsAsync(registrationFeeRequestInfo);

        _mockAccountsServiceClient
            .Setup(client => client.GetOrganisationNameById(registrationFeeRequestInfo.OrganisationId))
            .ReturnsAsync(organisationName);

        _mockAccountsServiceClient
            .Setup(client => client.GetNationDetailsById(registrationFeeRequestInfo.NationId))
            .ReturnsAsync(nationDetails);

        var paymentFeeResponse = _fixture.Create<PaymentFeeResponseDto>();

        _mockPaymentServiceClient
            .Setup(client => client.GetRegistrationPaymentFee(It.IsAny<PaymentFeeRequestDto>()))
            .ReturnsAsync(paymentFeeResponse);

        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        await _service.GetPaymentFeeDetailsByRegistrationMaterialId(id);

        // Assert
        _mockReprocessorExporterServiceClient.Verify(c => c.GetRegistrationFeeRequestByRegistrationMaterialId(id), Times.Once);
        _mockAccountsServiceClient.Verify(c => c.GetOrganisationNameById(registrationFeeRequestInfo.OrganisationId), Times.Once);
        _mockAccountsServiceClient.Verify(c => c.GetNationDetailsById(registrationFeeRequestInfo.NationId), Times.Once);
        _mockPaymentServiceClient.Verify(c => c.GetRegistrationPaymentFee(It.IsAny<PaymentFeeRequestDto>()), Times.Once);
    }

    [TestMethod]
    public async Task SaveOfflinePayment_ShouldReturnTrue_WhenClientCallSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestDto = _fixture.Create<OfflinePaymentRequestDto>();

        var expectedDto = new SaveOfflinePaymentRequestDto
        {
            Amount = requestDto.Amount,
            PaymentReference = requestDto.PaymentReference,
            PaymentDate = requestDto.PaymentDate,
            PaymentMethod = requestDto.PaymentMethod,
            Regulator = requestDto.Regulator,
            UserId = userId,
            Description = ReprocessorExporterConstants.OfflinePaymentRegistrationDescription,
            Comments = ReprocessorExporterConstants.OfflinePaymentRegistrationComment
        };

        _mockPaymentServiceClient
            .Setup(client => client.SaveOfflinePayment(It.Is<SaveOfflinePaymentRequestDto>(dto =>
                dto.Amount == expectedDto.Amount &&
                dto.PaymentReference == expectedDto.PaymentReference &&
                dto.PaymentDate == expectedDto.PaymentDate &&
                dto.PaymentMethod == expectedDto.PaymentMethod &&
                dto.Regulator == expectedDto.Regulator &&
                dto.UserId == expectedDto.UserId &&
                dto.Description == expectedDto.Description &&
                dto.Comments == expectedDto.Comments
            )))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SaveOfflinePayment(userId, requestDto);

        // Assert
        result.Should().BeTrue();
    }
    
    [TestMethod]
    public async Task GetPaymentFeeDetailsByAccreditationMaterialId_ShouldReturnMappedPaymentFeeDetailsDto_FromAccountAndPaymentServices()
    {
        // Arrange
        var id = 1;
        var accreditationFeeContextDto = _fixture.Create<AccreditationFeeContextDto>();
        var organisationName = "Green Ltd";

        var accreditationId = Guid.NewGuid();

        var nationDetails = new NationDetailsResponseDto { Name = "England", NationCode = "GB-ENG" };

        var paymentFee = 3000.00m;

        _mockReprocessorExporterServiceClient
            .Setup(client => client.GetAccreditationPaymentFeeDetailsByAccreditationId(It.IsAny<Guid>()))
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
                accreditationFeeContextDto.SubmittedDate,
                accreditationFeeContextDto.ApplicationType.ToString(),
                accreditationFeeContextDto.ApplicationReferenceNumber))
            .ReturnsAsync(paymentFee);

        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        // Act
        var result = await _service.GetAccreditationPaymentFeeDetailsByAccreditationId(accreditationId);

        // Assert
        result.Should().NotBeNull();
        result.AccreditationId.Should().Be(accreditationId);
        result.OrganisationName.Should().Be(organisationName);
        result.SiteAddress.Should().BeEquivalentTo(accreditationFeeContextDto.SiteAddress);
        result.ApplicationReferenceNumber.Should().Be(accreditationFeeContextDto.ApplicationReferenceNumber);
        result.MaterialName.Should().Be(accreditationFeeContextDto.MaterialName);
        result.ApplicationType.Should().Be(accreditationFeeContextDto.ApplicationType);
        result.SubmittedDate.Should().Be(accreditationFeeContextDto.SubmittedDate);
        result.FeeAmount.Should().Be(paymentFee);
    }
    
    [TestMethod]
    public async Task GetPaymentFeeDetailsByAccreditationMaterialId_ShouldThrowsException_WhenServiceFails()
    {
        var id = Guid.NewGuid();

        _mockReprocessorExporterServiceClient
            .Setup(client => client.GetAccreditationPaymentFeeDetailsByAccreditationId(id))
            .ThrowsAsync(new Exception("Service unavailable"));
        
        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        await FluentActions.Invoking(() => _service.GetAccreditationPaymentFeeDetailsByAccreditationId(id))
                            .Should().ThrowAsync<Exception>()
                            .WithMessage("Service unavailable");
    }

    [TestMethod]
    public async Task MarkAsDulyMadeByRegistrationMaterialId_ShouldReturnTrue_WhenClientCallSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var requestDto = _fixture.Create<MarkAsDulyMadeRequestDto>();

        var expectedDto = new MarkAsDulyMadeWithUserIdDto
        {
            DulyMadeDate = requestDto.DulyMadeDate,
            DeterminationDate = requestDto.DeterminationDate,
            DulyMadeBy = userId
        };

        _mockReprocessorExporterServiceClient
            .Setup(client => client.MarkAsDulyMadeByRegistrationMaterialId(id, It.Is<MarkAsDulyMadeWithUserIdDto>(dto =>
                dto.DulyMadeDate == expectedDto.DulyMadeDate &&
                dto.DeterminationDate == expectedDto.DeterminationDate &&
                dto.DulyMadeBy == expectedDto.DulyMadeBy
            )))
            .ReturnsAsync(true);

        // Act
        var result = await _service.MarkAsDulyMadeByRegistrationMaterialId(id, userId, requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetAccreditationsByRegistrationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<RegistrationOverviewDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetRegistrationByIdWithAccreditationsAsync(id, 2025))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetRegistrationByIdWithAccreditationsAsync(id, 2025);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task GetSamplingPlanByAccreditationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<AccreditationSamplingPlanDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetSamplingPlanByAccreditationId(id))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetSamplingPlanByAccreditationId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task MarkAccreditationStatusAsDulyMade_ShouldReturnTrue_WhenClientCallSucceeds()
    {
        // Arrange
        var accreditationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var requestDto = _fixture.Create<MarkAsDulyMadeRequestDto>();

        var expectedDto = new MarkAsDulyMadeRequestDto()
        {
            DulyMadeDate = requestDto.DulyMadeDate,
            DeterminationDate = requestDto.DeterminationDate

        };

        _mockReprocessorExporterServiceClient
            .Setup(client => client.MarkAsDulyMadeByAccreditationId(accreditationId, It.Is<MarkAsDulyMadeWithUserIdDto>(dto =>
                dto.DulyMadeDate == expectedDto.DulyMadeDate &&
                dto.DeterminationDate == expectedDto.DeterminationDate &&
                dto.DulyMadeBy == userId
            )))
            .ReturnsAsync(true);

        // Act
        var result = await _service.MarkAsDulyMadeByAccreditationId(accreditationId, userId, requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task UpdateAccreditationMaterialTaskStatus_ShouldReturnExpectedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestDto = _fixture.Create<UpdateAccreditationTaskStatusWithUserIdDto>();

        _mockReprocessorExporterServiceClient.Setup(client => client.UpdateRegulatorAccreditationTaskStatus(requestDto))
            .ReturnsAsync(true);

        _mockReprocessorExporterServiceClient.Setup(client => client.UpdateRegulatorAccreditationTaskStatus(It.Is<UpdateAccreditationTaskStatusWithUserIdDto>(dto =>
                        dto.AccreditationId == requestDto.AccreditationId &&
                        dto.TaskName == requestDto.TaskName &&
                        dto.Status == requestDto.Status &&
                        dto.Comments == requestDto.Comments &&
                        dto.UpdatedByUserId == userId)))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateRegulatorAccreditationTaskStatus(userId, requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task SaveAccreditationOfflinePayment_ShouldReturnTrue_WhenClientCallSucceeds()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requestDto = _fixture.Create<OfflinePaymentRequestDto>();

        var expectedDto = new SaveOfflinePaymentRequestDto
        {
            Amount = requestDto.Amount,
            PaymentReference = requestDto.PaymentReference,
            PaymentDate = requestDto.PaymentDate,
            PaymentMethod = requestDto.PaymentMethod,
            Regulator = requestDto.Regulator,
            UserId = userId,
            Description = ReprocessorExporterConstants.OfflinePaymentAccreditationDescription,
            Comments = ReprocessorExporterConstants.OfflinePaymentAccreditationComment
        };

        _mockPaymentServiceClient
            .Setup(client => client.SaveAccreditationOfflinePayment(It.Is<SaveOfflinePaymentRequestDto>(dto =>
                dto.Amount == expectedDto.Amount &&
                dto.PaymentReference == expectedDto.PaymentReference &&
                dto.PaymentDate == expectedDto.PaymentDate &&
                dto.PaymentMethod == expectedDto.PaymentMethod &&
                dto.Regulator == expectedDto.Regulator &&
                dto.UserId == expectedDto.UserId &&
                dto.Description == expectedDto.Description &&
                dto.Comments == expectedDto.Comments
            )))
            .ReturnsAsync(true);

        // Act
        var result = await _service.SaveAccreditationOfflinePayment(userId, requestDto);

        // Assert
        result.Should().BeTrue();
    }



    [TestMethod]
    public async Task SaveApplicationTaskQueryNotes_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<QueryNoteRequestDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.SaveApplicationTaskQueryNotes(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.SaveApplicationTaskQueryNotes(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), Guid.NewGuid(), requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task SaveRegistrationTaskQueryNotes_ShouldReturnExpectedResult()
    {
        // Arrange
        var requestDto = _fixture.Create<QueryNoteRequestDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.SaveRegistrationTaskQueryNotes(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), requestDto))
                   .ReturnsAsync(true);

        // Act
        var result = await _service.SaveRegistrationTaskQueryNotes(Guid.Parse("676b40a5-4b72-4646-ab39-8e3c85ccc175"), Guid.NewGuid(), requestDto);

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetBusinessPlanByAccreditationId_ShouldReturnExpectedResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expectedDto = _fixture.Create<AccreditationBusinessPlanDto>();
        _mockReprocessorExporterServiceClient.Setup(client => client.GetBusinessPlanByAccreditationId(id))
                   .ReturnsAsync(expectedDto);

        // Act
        var result = await _service.GetBusinessPlanByAccreditationId(id);

        // Assert
        result.Should().BeEquivalentTo(expectedDto);
    }

    [TestMethod]
    public async Task BusinessPlanByAccreditationId_ShouldThrowsException_WhenServiceFails()
    {
        var id = Guid.NewGuid();

        _mockReprocessorExporterServiceClient
            .Setup(client => client.GetBusinessPlanByAccreditationId(id))
            .ThrowsAsync(new Exception("Service unavailable"));

        _service = new ReprocessorExporterService(_mockReprocessorExporterServiceClient.Object, _mockAccountsServiceClient.Object, _mockPaymentServiceClient.Object);

        await FluentActions.Invoking(() => _service.GetBusinessPlanByAccreditationId(id))
                            .Should().ThrowAsync<Exception>()
                            .WithMessage("Service unavailable");
    }
}