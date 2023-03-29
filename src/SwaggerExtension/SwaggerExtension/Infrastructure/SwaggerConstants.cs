namespace TechBuddy.Extensions.OpenApi.Infrastructure;
internal class SwaggerConstants
{
    internal const string DefaultProjectName = "TechBuddy.Extensions.ProjectName";

    internal const string DefaultSwaggerEndpointFileName = "swagger.json";
    internal const string DefaultSwaggerEndpointFileDirectory = "swagger";
    internal const string DefaultSwaggerEndpoint = $"/{DefaultSwaggerEndpointFileDirectory}/v1/{DefaultSwaggerEndpointFileName}";
    internal const string DefaultSwaggerApiVersion = "1";

    internal const string DefaultResponseContentType = "application/json";

    internal const string BearerFormat = "JWT";
    internal const string BearerHeaderKey = "bearer";
    internal static string DefaultBearerDescription => $"JWT Authorization header using the {BearerHeaderKey} scheme. \r\n\r\n Enter your {BearerHeaderKey} token in the text input below.";
}
