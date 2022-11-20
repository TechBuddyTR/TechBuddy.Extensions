using ApiVersioningExtension.Tests.Infrastructure.Helpers;
using ApiVersioningExtension.Tests.Infrastructure.Models;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ApiVersioningExtension.Tests.Tests;
internal class ErrorResponseProviderTests
{
    [Test]
    public async Task ErrorResponse_WithNoErrorResponseProvider_ShoudReturnDefaultModel()
    {
        // Arrange
        var errorResponseTestServer = GetErrorResponseTestServer();
        var errorResponseClient = errorResponseTestServer.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=1.0");
        request.Headers.Add(TestConstants.ApiversionKey, "2.0");

        // Action 
        var response = await errorResponseClient.SendAsync(request);
        var respo = await response.Content.ReadAsStringAsync();
        var responseJsonObject = JsonNode.Parse(respo).AsObject().First().Value.AsObject();


        // Assert 
        responseJsonObject.ContainsKey("code").Should().BeTrue();
        responseJsonObject.ContainsKey("message").Should().BeTrue();
        responseJsonObject["code"].ToString().Should().Be("AmbiguousApiVersion");
    }


    [Test]
    public async Task ErrorResponse_WithCustomErrorResponseProvider_ShoudReturnCustomModel()
    {
        // Arrange
        var errorResponseTestServer = GetErrorResponseTestServer(useCustomProvider: true);
        var errorResponseClient = errorResponseTestServer.CreateClient();

        var expectedModel = new CustomErrorResponseModel()
        {
            ErrorMessage = "ApiVersioning Exception",
            ErrorCode = "AmbiguousApiVersion"
        };

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Test?{TestConstants.ApiversionKey}=1.0");
        request.Headers.Add(TestConstants.ApiversionKey, "2.0");

        // Action 
        var response = await errorResponseClient.SendAsync(request);

        var resultModel = await response.Content.ReadFromJsonAsync<CustomErrorResponseModel>();

        // Assert 
        resultModel.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(p => p.ErrorDetail));
    }


    #region Private Methods

    private TestServer GetErrorResponseTestServer(bool useCustomProvider = false)
    {
        var hostBuilder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddMvc(i => i.EnableEndpointRouting = false);

                services.AddTechBuddyApiVersioning(config =>
                {
                    config.AssumeDefaultVersionWhenUnspecified = true;

                    if (useCustomProvider)
                        config.UseErrorResponseProvider<CustomErrorResponseProvider>();

                    config.AddHeaderApiVersionReader(TestConstants.ApiversionKey);
                    config.AddQueryStringApiVersionReader(TestConstants.ApiversionKey);
                });
            })
            .Configure(app =>
            {
                app.UseMvc();
            });

        return new TestServer(hostBuilder);
    }

    #endregion
}
