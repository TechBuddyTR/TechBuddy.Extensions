# ApiVersioning

ApiVersioning Extension is aiming to add ApiVersioning Feature for your WebAPI Projects. It's configurable as well as it is configured with default values once this is implemented.


ApiVersioningConfig has three different functions, can be seen below, which is to add the ability to the system to read the provided api-version from different places.



## Usage

```csharp
builder.Services.AddTechBuddyApiVersioning(config =>
{
    config.AssumeDefaultVersionWhenUnspecified = true;  // To assume default version number will be used when it is not specified
    config.DefaultApiVersion = "1.0";                   // Default Api Version
    config.EnableVersionedApiExplorer = true;           // Allows it to be enabled in OpenAPI Supports such as Swagger. It returns back all the available api versions in your WebAPI project
    config.ReportApiVersions = true;                    // Allows all the available api versions for the specific endpoint to be returned in Response Header section with api-supported-versions key.

    config.AddUrlSegmentApiVersionReader();                  // Reads from Url Route -> https://localhost/api/v1/Users?id=5
    config.AddHeaderApiVersionReader("x-api-version");       // Reads from Header with provided key (x-api-version)
    config.AddQueryStringApiVersionReader("x-api-version");  // Reads from query string with provided key (x-api-version) -> https://localhost/api/v1/Users?x-api-version=1.0&id=5
});
```

It could be used like showed below. In this case, version 1.0 will be used as default version when not specified. And also ReportApiVersions will be true.
```csharp
builder.Services.AddTechBuddyApiVersioning();
```


If you injected ApiVersioning with `UrlSegmentReader`, your controller must have the `Route` parameter accordingly. 
However, if you use more than one reader and at least one of them is UrlSegmentReader (for instance; UrlSegment and Header), your controllers must have both route parameter with and without `v{version:apiVersion}`

```csharp
[ApiController]
[Route(ApiVersioningConfig.ControllerRoute)] // api/v{version:apiVersion}/[controller] -> this for UrlSegment
//[Route("api/[controller]")] -> this is for the others (Header, QueryString)
[ApiVersion("2.0")]
public class TestController : ControllerBase
{
    [HttpGet("GetTest")]
    public ActionResult Get()
    {
        return Ok("Test Success V2");
    }
}
```

----

## Usage with CustomErrorMessageResponseProvider

We are able to add multiple ApiVersionReader together. However, if the api version we provided is different than we provided on the other, such as v1 on QueryString but v2 on header, there will be an exception thrown.
When this exception is occured, the default response model will be provided by .Net ApiVersioning middleware. Nonetheless, we are also able to customize this response model.

To do that;

```csharp

internal class CustomErrorResponseProvider : IErrorResponseProvider
{
    public IActionResult CreateResponse(ErrorResponseContext context)
    {
        var response = new()
        {
            ErrorCode = context.ErrorCode,
            ErrorMessage = "ApiVersioning Exception",
            ErrorDetail = context.Message
        };

        var result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        return result;
    }
}

builder.Services.AddTechBuddyApiVersioning(config =>
{
    config.AddHeaderApiVersionReader("x-api-version");       // Reads from Header with provided key (x-api-version)
    config.AddQueryStringApiVersionReader("x-api-version");  // Reads from query string with provided key (x-api-version) -> https://localhost/api/v1/Users?x-api-version=1.0&id=5

    config.UseErrorResponseProvider(new CustomErrorResponseProvider());
    // or
    //config.UseErrorResponseProvider<CustomErrorResponseProvider>();
});
```