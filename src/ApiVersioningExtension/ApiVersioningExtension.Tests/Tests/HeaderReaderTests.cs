using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ApiVersioningExtension.Tests.Tests;

public class HeaderReaderTests
{
    private TestServer headerReaderTestServer;

    private HttpClient headerReaderClient;

    [OneTimeSetUp]
    public void Setup()
    {
        headerReaderTestServer = GetHeaderReaderTestServer();
        headerReaderClient = headerReaderTestServer.CreateClient();
    }

    [Test]
    public async Task ApiVersioningHeaderReader_WithV1_ShoudPass()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Test");
        request.Headers.Add(TestConstants.ApiversionKey, "1.0");

        // Action 
        HttpResponseMessage response = await headerReaderClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V1");
    }


    [Test]
    public async Task ApiVersioningHeaderReader_WithV2_ShoudPass()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Test");
        request.Headers.Add(TestConstants.ApiversionKey, "2.0");


        // Action 
        HttpResponseMessage response = await headerReaderClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V2");
    }




    [Test]
    public async Task ApiVersioningHeaderReader_WithNoVersion_ShoudReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Test");

        // Action 
        HttpResponseMessage response = await headerReaderClient.SendAsync(request);


        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ApiVersioningHeaderReader_WithInvalidVersion_ShoudReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Test");
        request.Headers.Add(TestConstants.ApiversionKey, "5.0");

        // Action 
        HttpResponseMessage response = await headerReaderClient.SendAsync(request);


        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }


    #region Private Methods

    private TestServer GetHeaderReaderTestServer()
    {
        if (headerReaderTestServer is not null)
            return headerReaderTestServer;

        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);

                services.AddTechBuddyApiVersioning(config =>
                {
                    config.AssumeDefaultVersionWhenUnspecified = false;

                    config.AddHeaderApiVersionReader(TestConstants.ApiversionKey);
                });
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        headerReaderTestServer = new TestServer(hostBuilder);

        return headerReaderTestServer;
    }

    #endregion
}