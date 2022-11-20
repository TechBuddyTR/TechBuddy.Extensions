using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ApiVersioningExtension.Tests.Tests;
internal class MultiReaderTests
{
    private TestServer multiReaderTestServer;

    private HttpClient multiReaderClient;


    [OneTimeSetUp]
    public void Setup()
    {
        multiReaderTestServer = GetMultiReaderTestServer();
        multiReaderClient = multiReaderTestServer.CreateClient();
    }


    [Test]
    public async Task MultiReader_WithNoVersion_ShoudReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Test");

        // Action 
        HttpResponseMessage response = await multiReaderClient.SendAsync(request);


        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task MultiReader_WithV1_ShoudPass()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=1.0");

        // Action 
        HttpResponseMessage response = await multiReaderClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();


        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V1");
    }


    [Test]
    public async Task MultiReader_WithDifferentVersions_ShoudReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=1.0");
        request.Headers.Add(TestConstants.ApiversionKey, "2.0");

        // Action 
        var response = await multiReaderClient.SendAsync(request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task MultiReader_WithSameVersions_ShoudReturnOk()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=2.0");
        request.Headers.Add(TestConstants.ApiversionKey, "2.0");

        // Action 
        var response = await multiReaderClient.SendAsync(request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }



    #region Private Methods

    private TestServer GetMultiReaderTestServer()
    {
        if (multiReaderTestServer is not null)
            return multiReaderTestServer;

        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);

                services.AddTechBuddyApiVersioning(config =>
                {
                    config.AssumeDefaultVersionWhenUnspecified = false;

                    config.AddHeaderApiVersionReader(TestConstants.ApiversionKey);
                    config.AddQueryStringApiVersionReader(TestConstants.ApiversionKey);
                });
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        multiReaderTestServer = new TestServer(hostBuilder);

        return multiReaderTestServer;
    }

    #endregion


}
