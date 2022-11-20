using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ApiVersioningExtension.Tests.Tests;
public sealed class GeneralTests
{
    private TestServer defaultVersionTestServer;

    [OneTimeSetUp]
    public void SetUp()
    {
        defaultVersionTestServer = GetDefaultVersionTestServer();
    }


    #region DefaultVersion Tests With Config

    [Test]
    public async Task WhenAssumeDefaultVersionWhenUnspecified_TRUE_ShouldUseDefaultVersion()
    {
        // Arrange
        var client = defaultVersionTestServer.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        responseContent.Should().Be("V2");
    }

    [Test]
    public async Task WhenAssumeDefaultVersionWhenUnspecified_FALSE_ShouldReturnBadRequest()
    {
        // Arrange
        var server = GetDefaultVersionTestServer("1.0", false);
        var client = server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task WhenAssumeDefaultVersionWhenUnspecified_FALSE_ShouldUseNOTDefaultVersion()
    {
        // Arrange
        var server = GetDefaultVersionTestServer("1.0", false);
        var client = server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=2.0");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V2");
    }

    #endregion

    #region DefaultVersion Tests With Default Config

    [Test]
    public async Task DefaultConfig_WithNoApiVersioning_ShouldUseDefaultVersion()
    {
        // Arrange
        var server = GetTestServerDefaultConfig();
        var client = server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        //Assert
        responseContent.Should().Be("V1");
    }

    [Test]
    public async Task DefaultConfig_WithNoApiVersioning_ShouldReturnOk()
    {
        // Arrange
        var server = GetTestServerDefaultConfig();
        var client = server.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test");

        // Action
        HttpResponseMessage response = await client.SendAsync(request);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    #endregion



    #region Private Methods

    private static TestServer GetDefaultVersionTestServer(string defaultVersion = "2.0", bool assume = true)
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);

                services.AddTechBuddyApiVersioning(config =>
                {
                    config.AddQueryStringApiVersionReader(TestConstants.ApiversionKey);
                    config.AssumeDefaultVersionWhenUnspecified = assume;
                    config.DefaultApiVersion = defaultVersion;
                });
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        return new TestServer(hostBuilder);
    }

    private static TestServer GetTestServerDefaultConfig()
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);

                services.AddTechBuddyApiVersioning();
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        return new TestServer(hostBuilder);
    }

    #endregion
}
