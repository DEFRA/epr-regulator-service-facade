using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using Moq.Protected;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPR.RegulatorService.Facade.Core.Clients;

namespace EPR.RegulatorService.Facade.UnitTests.Clients;

[TestClass]
public class BaseHttpClientTests
{
    private Mock<HttpMessageHandler> _handlerMock = null!;
    private HttpClient _httpClient = null!;
    private TestableBaseHttpClient _client = null!;
    private const string Url = "http://test-api.com/endpoint";

    [TestInitialize]
    public void TestInitialize()
    {
        _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_handlerMock.Object);
        _client = new TestableBaseHttpClient(_httpClient);
    }

    [TestMethod]
    public async Task GetAsync_Should_CallGet_AndReturnDeserializedResult()
    {
        var responseObj = new TestResponse { Result = "success" };
        var json = SerializeCamelCase(responseObj);

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri!.ToString() == Url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var result = await _client.PublicGet<TestResponse>(Url);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Result.Should().Be("success");
        }
    }

    [TestMethod]
    public async Task PostAsync_Should_CallPost_AndReturnDeserializedResult()
    {
        var request = new TestRequest { Id = 1, Name = "Test" };
        var response = new TestResponse { Result = "success" };
        var json = SerializeCamelCase(response);

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString() == Url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var result = await _client.PublicPost<TestRequest, TestResponse>(Url, request);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Result.Should().Be("success");
        }
    }

    [TestMethod]
    public async Task PutAsync_Should_CallPut_AndReturnDeserializedResult()
    {
        var request = new TestRequest { Id = 1, Name = "Test" };
        var response = new TestResponse { Updated = true };
        var json = SerializeCamelCase(response);

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Put &&
                    req.RequestUri!.ToString() == Url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var result = await _client.PublicPut<TestRequest, TestResponse>(Url, request);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result!.Updated.Should().BeTrue();
        }
    }

    [TestMethod]
    public async Task PatchAsync_Should_CallPatch_AndReturnTrueOnSuccess()
    {
        var request = new TestRequest { Id = 1, Name = "PatchTest" };

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method.Method == "PATCH" &&
                    req.RequestUri!.ToString() == Url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var result = await _client.PublicPatch(Url, request);

        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteAsync_Should_CallDelete_AndReturnTrueOnSuccess()
    {
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Delete &&
                    req.RequestUri!.ToString() == Url),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var result = await _client.PublicDelete(Url);

        result.Should().BeTrue();
    }

    private static string SerializeCamelCase<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private class TestableBaseHttpClient : BaseHttpClient
    {
        public TestableBaseHttpClient(HttpClient httpClient) : base(httpClient) { }

        public Task<T> PublicGet<T>(string url) => GetAsync<T>(url);
        public Task<T?> PublicPost<TReq, T>(string url, TReq data) => PostAsync<TReq, T>(url, data);
        public Task<T> PublicPut<TReq, T>(string url, TReq data) => PutAsync<TReq, T>(url, data);
        public Task<bool> PublicPatch<TReq>(string url, TReq data) => PatchAsync(url, data);
        public Task<bool> PublicDelete(string url) => DeleteAsync(url);
    }

    public class TestRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TestResponse
    {
        public string Result { get; set; } = string.Empty;
        public bool Updated { get; set; }
    }
}
