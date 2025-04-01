using System;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks;
using EPR.RegulatorService.Facade.Core.Clients;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace EPR.RegulatorService.Facade.UnitTests.Core.Clients;

[TestClass]
public class BaseHttpClientTests
{
    private Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private HttpClient _httpClient;
    private TestHttpClient _client;

    public record TestDto(string Message);

    // Simple subclass since BaseHttpClient is abstract
    private class TestHttpClient : BaseHttpClient
    {
        public TestHttpClient(HttpClient httpClient) : base(httpClient) { }

        public new Task<T> GetAsync<T>(string url) => base.GetAsync<T>(url);
        public new Task<TRes> PostAsync<TReq, TRes>(string url, TReq req) => base.PostAsync<TReq, TRes>(url, req);
        public new Task<TRes> PutAsync<TReq, TRes>(string url, TReq req) => base.PutAsync<TReq, TRes>(url, req);
        public new Task<bool> PatchAsync<TReq>(string url, TReq req) => base.PatchAsync(url, req);
        public new Task<bool> DeleteAsync(string url) => base.DeleteAsync(url);
    }

    [TestInitialize]
    public void Setup()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://test.api")
        };
        _client = new TestHttpClient(_httpClient);
    }

    [TestMethod]
    public async Task GetAsync_ReturnsDeserializedObject_OnSuccess()
    {
        var url = "/test";
        var expected = new TestDto("hello");
        var json = JsonSerializer.Serialize(expected, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        SetupHttpHandler(HttpMethod.Get, url, json, HttpStatusCode.OK);

        var result = await _client.GetAsync<TestDto>(url);

        result.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public async Task PostAsync_ReturnsDeserializedResponse_OnSuccess()
    {
        var url = "/post";
        var request = new TestDto("send");
        var response = new TestDto("received");
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        SetupHttpHandler(HttpMethod.Post, url, json, HttpStatusCode.Created);

        var result = await _client.PostAsync<TestDto, TestDto>(url, request);

        result.Should().BeEquivalentTo(response);
    }

    [TestMethod]
    public async Task PutAsync_ReturnsDeserializedResponse_OnSuccess()
    {
        var url = "/put";
        var request = new TestDto("update");
        var response = new TestDto("updated");
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        SetupHttpHandler(HttpMethod.Put, url, json, HttpStatusCode.OK);

        var result = await _client.PutAsync<TestDto, TestDto>(url, request);

        result.Should().BeEquivalentTo(response);
    }

    [TestMethod]
    public async Task PatchAsync_ReturnsTrue_OnSuccess()
    {
        var url = "/patch";
        var request = new TestDto("partial");

        SetupHttpHandler(HttpMethod.Patch, url, "", HttpStatusCode.NoContent);

        var result = await _client.PatchAsync(url, request);

        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task DeleteAsync_ReturnsTrue_OnSuccess()
    {
        var url = "/delete";

        SetupHttpHandler(HttpMethod.Delete, url, "", HttpStatusCode.OK);

        var result = await _client.DeleteAsync(url);

        result.Should().BeTrue();
    }

    [TestMethod]
    public async Task GetAsync_ThrowsException_OnNonSuccess()
    {
        var url = "/fail";
        SetupHttpHandler(HttpMethod.Get, url, "Error", HttpStatusCode.InternalServerError);

        Func<Task> act = async () => await _client.GetAsync<TestDto>(url);

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    private void SetupHttpHandler(HttpMethod method, string url, string content, HttpStatusCode statusCode)
    {
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == method &&
                    req.RequestUri.ToString() == $"https://test.api{url}"),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            })
            .Verifiable();
    }
}
