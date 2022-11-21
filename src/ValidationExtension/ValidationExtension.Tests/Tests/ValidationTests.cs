using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using TechBuddy.Extension.Validation.Extensions;
using TechBuddy.Extensions.Tests.Common.TestCommon.Builders;
using ValidationExtension.Tests.Infrastructure.Helpers.ModelProviders;

namespace ValidationExtension.Tests.Tests;
internal class ValidationTests
{
    private TestServer defaultVersionTestServer;

    [OneTimeSetUp]
    public void SetUp()
    {
        defaultVersionTestServer = GetTestServerWithDefaultConfig();
    }

    [Test]
    public async Task TestController_WithTestModel_ShouldReturnModelName()
    {
        // Arrange
        var client = defaultVersionTestServer.CreateClient();
        var testModel = new TestModel()
        {
            Id = 1,
            Name = "Test"
        };

        var request = HttpRequestBuilder
                        .CreateRequest("api/Test", HttpMethod.Post)
                        .SetBody(testModel)
                        .ToRequestMessage();

        // Action
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        responseContent.Should().Be(testModel.Name);
    }

    [Test]
    public async Task TestController_WithInvalidTestModel_ShouldReturnBadRequest()
    {
        // Arrange
        var client = defaultVersionTestServer.CreateClient();
        var testModel = new TestModel()
        {
            Id = 0,
            Name = "Test"
        };

        var request = HttpRequestBuilder
                        .CreateRequest("api/Test", HttpMethod.Post)
                        .SetBody(testModel)
                        .ToRequestMessage();

        // Action
        HttpResponseMessage response = await client.SendAsync(request);

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task TestController_WithModelProvider_ShouldReturnTestResponse()
    {
        // Arrange
        var client = GetTestServerModelProvider().CreateClient();
        var testModel = new TestModel()
        {
            Id = 0,
            Name = "Test"
        };

        var request = HttpRequestBuilder
                        .CreateRequest("api/Test", HttpMethod.Post)
                        .SetBody(testModel)
                        .ToRequestMessage();

        // Action
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadFromJsonAsync<TestResponse>();

        //Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        responseContent.Should().NotBeNull();
    }


    [Test]
    public async Task TestController_WithSkipValidationAndInvalidModel_ShouldSkip()
    {
        // Arrange
        var client = GetTestServerModelProvider().CreateClient();
        var testModel = new TestModel()
        {
            Id = 0,
            Name = "Test"
        };

        var request = HttpRequestBuilder
                        .CreateRequest("api/Test/SkipValidation", HttpMethod.Post)
                        .SetBody(testModel)
                        .ToRequestMessage();

        // Action
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        responseContent.Should().Be(testModel.Name);
    }

    [Test]
    public async Task TestController_WithSkipValidationAndValidModel_ShouldReturnModelName()
    {
        // Arrange
        var client = GetTestServerModelProvider().CreateClient();
        var testModel = new TestModel()
        {
            Id = 1,
            Name = "Test"
        };

        var request = HttpRequestBuilder
                        .CreateRequest("api/Test/SkipValidation", HttpMethod.Post)
                        .SetBody(testModel)
                        .ToRequestMessage();

        // Action
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        responseContent.Should().Be(testModel.Name);
    }



    private static TestServer GetTestServerWithDefaultConfig()
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);
                services.AddTechBuddyValidator();
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        return new TestServer(hostBuilder);
    }

    private static TestServer GetTestServerModelProvider()
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);
                services.AddTechBuddyValidator(conf => conf.UseModelProvider<TestModelProvider>());
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        return new TestServer(hostBuilder);
    }

}
