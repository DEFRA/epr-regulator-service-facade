using System.Net;
using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.API.Controllers;
using EPR.RegulatorService.Facade.API.Shared;
using EPR.RegulatorService.Facade.Core.Services.Regulator;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using EPR.RegulatorService.Facade.UnitTests.API.MockData;

namespace EPR.RegulatorService.Facade.UnitTests.API.Shared
{
    [TestClass]
    public class RegulatorUsersTests
    {
        private readonly Mock<ILogger<SubmissionsController>> _loggerMock = new();
        private readonly Mock<IRegulatorOrganisationService> _regulatorOrganisationServiceMock = new();
        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private readonly Guid _userId = Guid.NewGuid();
        private readonly Guid _organisationId = Guid.NewGuid();
        private RegulatorUsers<SubmissionsController> _SubmissionsControllerRegulatorUsers;
        
        [TestInitialize]
        public void Setup()
        {
            _SubmissionsControllerRegulatorUsers = new RegulatorUsers<SubmissionsController>(_regulatorOrganisationServiceMock.Object, _loggerMock.Object);
        }
    
        [TestMethod]
        public async Task When_Get_Regulator_Users_For_Organisation_should_return_Users()
        {
            // Arrange
            var handlerResponse =
                _fixture
                    .Build<HttpResponseMessage>()
                    .With(x => x.StatusCode, HttpStatusCode.NoContent)
                    .With(x => x.Content, new StringContent(GetRegulatorUsers()))
                    .Create();

            _regulatorOrganisationServiceMock
                .Setup(x => x.GetRegulatorUserList(_userId, _organisationId, true))
                .ReturnsAsync(handlerResponse);
            
            // Act
            var result = await _SubmissionsControllerRegulatorUsers.GetRegulatorUsers(_userId, _organisationId);

            // Assert
            result.Count.Should().Be(2);
        }
        
        private static string GetRegulatorUsers()
        {
            return JsonSerializer.Serialize(RegulatorUsersMockData.GetRegulatorUsers());
        }
    }
}