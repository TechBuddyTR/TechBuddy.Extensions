using ExceptionHandlingExtension.Tests.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using TechBuddy.Extensions.AspNetCore.ExceptionHandling;
using TechBuddy.Extensions.AspNetCore.ExceptionHandling.Infrastructure.Models;
using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ExceptionHandlingExtension.Tests;
public sealed class ExceptionHandlingTests
{
    [Test]
    public async Task ExceptionHandlingWithNoHandler_WithNoDetails_ShouldReturnInternalServerError()
    {
        // Arrange
        var server = GetServerWithOptions(new ExceptionHandlingOptions());
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task ExceptionHandlingWithNoHandler_WhenUseExceptionDetailsIsFalse_ShouldReturnInternalServerErrorMessage()
    {
        // Arrange
        var server = GetServerWithOptions(new ExceptionHandlingOptions());
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        var resObj = JsonSerializer.Deserialize<DefaultExceptionHandlerResponseModel>(responseContent, GeneralConstants.JsonOptions);

        resObj.Should().NotBeNull();
        resObj.Detail.Should().Be(TestConstants.DefaultExceptionMessage);
    }

    [Test]
    public async Task ExceptionHandlingWithNoHandler_WhenUseExceptionDetailsIsTrue_ShouldReturnExceptionMessage()
    {
        // Arrange
        var options = new ExceptionHandlingOptions() { UseExceptionDetails = true };
        options.UseLogger();
        var server = GetServerWithOptions(options);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();


        // Assert
        var resObj = JsonSerializer.Deserialize<DefaultExceptionHandlerResponseModel>(responseContent, GeneralConstants.JsonOptions);

        resObj.Should().NotBeNull();
        resObj.Detail.Should().Contain(TestConstants.ExceptionMessage);
    }

    [Test]
    public async Task ExceptionHandlingWithNoHandler_WithDetails_ShouldReturnInternalServer()
    {
        // Arrange
        var options = new ExceptionHandlingOptions() { UseExceptionDetails = true };
        options.UseLogger();
        var server = GetServerWithOptions(options);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);


        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task ExceptionHandlingWithNoHandler_WithDetails_ShouldReturnExceptionMessage()
    {
        // Arrange
        var options = new ExceptionHandlingOptions() { UseExceptionDetails = true };
        options.UseLogger();
        var server = GetServerWithOptions(options);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);

        var resObj = JsonSerializer.Deserialize<DefaultExceptionHandlerResponseModel>(responseContent, GeneralConstants.JsonOptions);

        resObj.Should().NotBeNull();
        resObj.Detail.Should().StartWith($"System.Exception: {TestConstants.ExceptionMessage}");
    }

    [Test]
    public async Task ExceptionHandling_WithHandler_ShouldReturnExceptionMessage()
    {
        // Arrange && Assert 
        var opt = new ExceptionHandlingOptions();
        opt.UseCustomHandler(async (ctx, ex, logger) =>
        {
            ex.Message.Should().NotBeNull();
            ex.Message.Should().Be(TestConstants.ExceptionMessage);

            await Task.CompletedTask; // expects returning task
        });

        var server = GetServerWithOptions(opt);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);
    }


    [Test]
    public async Task ExceptionHandlingWithHandler_WhenWriteToResponse_ShouldReturnTheMessage()
    {
        // Arrange && Assert
        var opt = new ExceptionHandlingOptions();
        opt.UseCustomHandler(async (ctx, ex, logger) =>
        {
            ex.Message.Should().NotBeNull();
            ex.Message.Should().Be(TestConstants.ExceptionMessage);

            // writes as json, so it's like "TestConstants.ExceptionMessage"
            await ctx.WriteResponseAsync(TestConstants.ExceptionMessage, System.Net.HttpStatusCode.InternalServerError);
        });

        var server = GetServerWithOptions(opt);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
        responseContent.Should().Be($"\"{TestConstants.ExceptionMessage}\"");
    }


    [Test]
    public async Task ExceptionHandlingWithNoHandler_WithCustomLogger_ShouldCallLogError()
    {
        // Arrange
        Mock<ILogger> mockLogger = new Mock<ILogger>();
        var opt = new ExceptionHandlingOptions();
        opt.UseLogger(mockLogger.Object);
        var server = GetServerWithOptions(opt);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action 
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        mockLogger.Verify(m => m.Log(LogLevel.Error,
                                     It.IsAny<EventId>(),
                                     It.Is<It.IsAnyType>((v, t) => true),
                                     It.IsAny<Exception>(),
                                     It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Test]
    public async Task ExceptionHandlingWithNoHandler_WithCustomLogger_ShouldCallLogErrorWithMessageDetails()
    {
        // Arrange
        var expectedMessage = TestConstants.ExceptionMessage;
        Mock<ILogger> mockLogger = new Mock<ILogger>();
        var opt = new ExceptionHandlingOptions() { UseExceptionDetails = true };
        opt.UseLogger(mockLogger.Object);
        var server = GetServerWithOptions(opt);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        mockLogger.Verify(logger => logger.Log(LogLevel.Error,
                                               It.IsAny<EventId>(),
                                               It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(expectedMessage)),
                                               It.IsAny<Exception>(),
                                               (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                          Times.Once);
    }

    [Test]
    public async Task ExceptionHandling_WithSingleCustomHandler_ShouldBeCalled()
    {
        // Assert
        var functionWasInvoked = false;
        var expectedMessage = TestConstants.ExceptionMessage;
        var options = new ExceptionHandlingOptions() { UseExceptionDetails = true };
        options.UseLogger();

        options.AddCustomHandler<Exception>((context, ex, logger) =>
        {
            functionWasInvoked = true;
            ex.Message.Should().Be(expectedMessage);
            return Task.CompletedTask;
        });
        
        var server = GetServerWithOptions(options);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowException");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        functionWasInvoked.Should().BeTrue();
    }

    [Test]
    public async Task ExceptionHandlingWithCustomHandler_WithUseHandler_ShouldBeCalled()
    {
        // Assert
        bool functionWasInvoked = false;
        var options = new ExceptionHandlingOptions();

        options.UseCustomHandler((context, ex, logger) =>
        {
            functionWasInvoked = false;
            return Task.CompletedTask;
        });

        options.AddCustomHandler<InvalidOperationException>((context, ex, logger) =>
        {
            functionWasInvoked = true;
            return Task.CompletedTask;
        });

        var server = GetServerWithOptions(options);
        var client = server.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test/ThrowCustomException");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);

        // Assert
        functionWasInvoked.Should().BeTrue();
    }

    #region Private Methods

    private static TestServer GetServerWithOptions(ExceptionHandlingOptions options)
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);
            })
            .Configure(async (app) =>
            {
                var env = app.ApplicationServices.GetService<IWebHostEnvironment>();

                await app.ConfigureTechBuddyExceptionHandling(options);

                app.UseMvc();
            });

        return new TestServer(hostBuilder);
    }

    #endregion
}
