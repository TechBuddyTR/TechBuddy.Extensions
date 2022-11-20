namespace ApiVersioningExtension.Tests.Tests;
public sealed class UrlSegmentReaderTests
{
    private TestServer urlSegmentReaderTestServer;

    private HttpClient urlSegmentReaderClient;

    [OneTimeSetUp]
    public void Setup()
    {
        urlSegmentReaderTestServer = GetUrlSegmentReaderTestServer();
        urlSegmentReaderClient = urlSegmentReaderTestServer.CreateClient();
    }


    [Test]
    public async Task ApiVersioningUrlSegmentReader_WithV1_ShoudPass()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/UrlSegmentTest");

        // Action 
        HttpResponseMessage response = await urlSegmentReaderClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();


        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V1");
    }

    [Test]
    public async Task ApiVersioningUrlSegmentReader_WithV2_ShoudPass()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v2/UrlSegmentTest");


        // Action 
        HttpResponseMessage response = await urlSegmentReaderClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V2");
    }

    [Test]
    public async Task ApiVersioningUrlSegmentReader_WithInvalidVersion_ShoudReturnNotFound()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/UrlSegmentTest");


        // Action 
        HttpResponseMessage response = await urlSegmentReaderClient.SendAsync(request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }


    [Test]
    public async Task ApiVersioningUrlSegmentReader_WithNoVersion_ShoudReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/v5/UrlSegmentTest");

        // Action 
        HttpResponseMessage response = await urlSegmentReaderClient.SendAsync(request);


        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }



    #region Private Methods

    private TestServer GetUrlSegmentReaderTestServer()
    {
        if (urlSegmentReaderTestServer is not null)
            return urlSegmentReaderTestServer;

        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);

                services.AddTechBuddyApiVersioning(config =>
                {
                    config.AssumeDefaultVersionWhenUnspecified = false;

                    config.AddUrlSegmentApiVersionReader();
                });
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        urlSegmentReaderTestServer = new TestServer(hostBuilder);

        return urlSegmentReaderTestServer;
    }

    #endregion
}
