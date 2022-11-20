using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ApiVersioningExtension.Tests.Tests;

public class QueryStringReaderTests
{

    private TestServer queryStringReaderTestServer;

    private HttpClient queryStringReaderClient;

    [OneTimeSetUp]
    public void Setup()
    {
        queryStringReaderTestServer = GetQueryStringReaderTestServer();
        queryStringReaderClient = queryStringReaderTestServer.CreateClient();
    }


    [Test]
    public async Task ApiVersioningQueryStringReader_WithV1_ShoudPass()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=1.0");

        // Action 
        HttpResponseMessage response = await queryStringReaderClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();


        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V1");
    }

    [Test]
    public async Task ApiVersioningQueryStringReader_WithV2_ShoudPass()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=2.0");


        // Action 
        HttpResponseMessage response = await queryStringReaderClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("V2");
    }

    [Test]
    public async Task ApiVersioningQueryStringReader_WithInvalidVersion_ShoudReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=5.0");


        // Action 
        HttpResponseMessage response = await queryStringReaderClient.SendAsync(request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }


    [Test]
    public async Task ApiVersioningQueryStringReader_WithNoVersion_ShoudReturnBadRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Test");

        // Action 
        HttpResponseMessage response = await queryStringReaderClient.SendAsync(request);


        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }



    #region Private Methods

    private TestServer GetQueryStringReaderTestServer()
    {
        if (queryStringReaderTestServer is not null)
            return queryStringReaderTestServer;

        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);

                services.AddTechBuddyApiVersioning(config =>
                {
                    config.AssumeDefaultVersionWhenUnspecified = false;

                    config.AddQueryStringApiVersionReader(TestConstants.ApiversionKey);
                });
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        queryStringReaderTestServer = new TestServer(hostBuilder);

        return queryStringReaderTestServer;
    }

    #endregion
}