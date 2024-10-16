using AutoFixture;
using AutoFixture.AutoMoq;
using EPR.RegulatorService.Facade.Core.Configs;
using EPR.RegulatorService.Facade.Core.Models.Requests.Registrations;
using EPR.RegulatorService.Facade.Core.Services.CommonData.ConcreteData;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Threading.Tasks;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Services.CommonData.ConcreteData
{

    [TestClass]
    public class CommonDataOrganisationRegistrationHandlerTests
    {
        private CommonDataOrganisationRegistrationHandler _sut ;

        private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
        private IOptions<CommonDataApiConfig> _configuration = default!;
        private const string BaseAddress = "http://localhost";
        private const string GetOrganisationRegistrations = "GetOrganisationRegistrations";
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock = new();
        private HttpClient _httpClient;
        private string _expectedUrl;

        [TestInitialize]
        public void Setup()
        {
            _configuration = Options.Create(new CommonDataApiConfig()
            {
                BaseUrl = BaseAddress,
                Timeout = 0,
                ServiceRetryCount = 0,
                Endpoints = new()
                {
                    GetOrganisationRegistrations = GetOrganisationRegistrations
                }
            });
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri(BaseAddress)
            };

            _sut = new(_httpClient, _configuration.Value);
        }

        [TestMethod]
        public async ValueTask When_Called_Expect_The_URL_To_be_Visited()
        {
            GetOrganisationRegistrationRequest request = new GetOrganisationRegistrationRequest()
            {
                PageNumber = 1,
                PageSize = 1
            };

            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationRegistrations}";
            SetupApiSuccessCall();

            // Act
            var response = await _sut.GetOrganisationRegistrations(request);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Post);
            response.IsSuccessStatusCode.Should().BeTrue();
        }

        [TestMethod]
        public async Task Should_return_bad_request_when_fetching_fails()
        {
            GetOrganisationRegistrationRequest request = new GetOrganisationRegistrationRequest()
            {
                PageNumber = 1,
                PageSize = 1
            };

            _expectedUrl = $"{BaseAddress}/{_configuration.Value.Endpoints.GetOrganisationRegistrations}";
            SetupApiBadRequestCall();

            var response = await _sut.GetOrganisationRegistrations(request);

            // Assert
            VerifyApiCall(_expectedUrl, HttpMethod.Post);
            response.IsSuccessStatusCode.Should().BeFalse();
        }

        private void SetupApiSuccessCall()
        {
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.OK)
                .Create();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();
        }

        private void SetupApiBadRequestCall()
        {
            var apiResponse = _fixture
                .Build<HttpResponseMessage>()
                .With(x => x.StatusCode, HttpStatusCode.BadRequest)
                .Create();

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(x => x.RequestUri != null && x.RequestUri.ToString() == _expectedUrl),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(apiResponse).Verifiable();
        }

        private void VerifyApiCall(string expectedUrl, HttpMethod method)
        {
            _httpMessageHandlerMock.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(
                    req => req.Method == method &&
                           req.RequestUri != null &&
                           req.RequestUri.ToString() == expectedUrl),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
