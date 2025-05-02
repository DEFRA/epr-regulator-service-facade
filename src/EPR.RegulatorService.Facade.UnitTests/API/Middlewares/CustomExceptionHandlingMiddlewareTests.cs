using EPR.RegulatorService.Facade.API.Middlewares;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Text.Json;

namespace EPR.RegulatorService.Facade.UnitTests.API.Middlewares;

[TestClass]
public class CustomExceptionHandlingMiddlewareTests
{
    private Mock<RequestDelegate> _nextMock;
    private Mock<ILogger<CustomExceptionHandlingMiddleware>> _loggerMock;
    private CustomExceptionHandlingMiddleware _middleware;

    [TestInitialize]
    public void TestInitialize()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<CustomExceptionHandlingMiddleware>>();
        _middleware = new CustomExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task InvokeAsync_ValidationException_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
                 .ThrowsAsync(new FluentValidation.ValidationException("Validation failed"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        using (new AssertionScope())
        {
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            context.Response.ContentType.Should().Be("application/json; charset=utf-8");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);
            response.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
            response.GetProperty("title").GetString().Should().Be("One or more validation errors occurred.");
        }
    }

    [TestMethod]
    public async Task InvokeAsync_InvalidOperationException_ReturnsBadRequest()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
                 .ThrowsAsync(new InvalidOperationException("Invalid operation"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        using (new AssertionScope())
        {
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            context.Response.ContentType.Should().Be("application/json; charset=utf-8");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);
            response.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.BadRequest);
            response.GetProperty("title").GetString().Should().Be("An invalid operation occurred.");
        }
    }

    [TestMethod]
    public async Task InvokeAsync_HttpRequestException_ReturnsInternalServerError()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
                 .ThrowsAsync(new HttpRequestException("HTTP request failed"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        using (new AssertionScope())
        {
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            context.Response.ContentType.Should().Be("application/json; charset=utf-8");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);
            response.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.InternalServerError);
            response.GetProperty("title").GetString().Should().Be("An HTTP request exception occurred.");
        }
    }

    [TestMethod]
    public async Task InvokeAsync_KeyNotFoundException_ReturnsNotFound()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
                 .ThrowsAsync(new KeyNotFoundException("Resource not found"));

        // Act
        await _middleware.InvokeAsync(context);

        // Assert
        using (new AssertionScope())
        {
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            context.Response.ContentType.Should().Be("application/json; charset=utf-8");

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await JsonSerializer.DeserializeAsync<JsonElement>(context.Response.Body);
            response.GetProperty("status").GetInt32().Should().Be((int)HttpStatusCode.NotFound);
            response.GetProperty("title").GetString().Should().Be("The requested resource could not be found.");
        }
    }

    [TestMethod]
    public async Task InvokeAsync_UnhandledException_ThrowsException()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
                 .ThrowsAsync(new Exception("Unexpected error"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<Exception>(async () => await _middleware.InvokeAsync(context));
    }

}

