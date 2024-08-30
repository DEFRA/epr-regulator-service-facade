using System.Net;
using System.Net.Http.Headers;
using Moq;
using Moq.Protected;

namespace EPR.RegulatorService.Facade.UnitTests.API.Support.Extensions
{
    public static class HttpMessageHandlerMockExtensions
    {
        public static void RespondWith(
            this Mock<HttpMessageHandler> messageHandlerMock,
            HttpStatusCode statusCode,
            HttpContent content)
        {
            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = content
                });
        }

        public static void VerifyRequest(
            this Mock<HttpMessageHandler> handlerMock,
            HttpMethod expectedMethod,
            Uri expectedUri,
            Times times)
        {
            handlerMock.Protected()
                .Verify(
                    "SendAsync",
                    times,
                    ItExpr.Is<HttpRequestMessage>(
                        req => req.Method == expectedMethod
                               && req.RequestUri == expectedUri),
                    ItExpr.IsAny<CancellationToken>());
        }

        public static void VerifyRequest(
            this Mock<HttpMessageHandler> handlerMock,
            HttpMethod expectedMethod,
            Uri expectedUri,
            Dictionary<string, string> expectedHeaders,
            Times expectedInvocationCount)
        {
            handlerMock.Protected()
                .Verify(
                    "SendAsync",
                    expectedInvocationCount,
                    ItExpr.Is<HttpRequestMessage>(x =>
                        x.Method == expectedMethod
                        && x.RequestUri == expectedUri
                        && HeadersMatch(x.Headers, expectedHeaders)),
                    ItExpr.IsAny<CancellationToken>());
        }

        private static bool HeadersMatch(HttpRequestHeaders headers, Dictionary<string, string> expectedHeaderDictionary)
        {
            var actualHeaderDictionary = headers.ToDictionary(h => h.Key, h => string.Join(";", h.Value));
            return Equals(actualHeaderDictionary, expectedHeaderDictionary)
                   || (actualHeaderDictionary.Count == expectedHeaderDictionary.Count
                       && !actualHeaderDictionary.Except(expectedHeaderDictionary).Any());
        }
    }
}