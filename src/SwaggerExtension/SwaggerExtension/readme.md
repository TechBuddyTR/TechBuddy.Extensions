# Swagger Extension

Swagger Extension is aiming to implement the Swagger and Swagger UI with all default configs and additionally some another features. 

### Usage 1

The basic implementation is to implement default Swagger and SwaggerUI.

```csharp
builder.Services.ConfigureTechBuddySwagger();
```

### Usage 2

```csharp
builder.Services.ConfigureTechBuddySwagger(config =>
{
    config.ProjectName = "TechBuddy.Extensions"; // The project name that'll be used on Swagger UI and Swagger documentation
    
    // To enable Bearer Authentication for Swagger UI
    config.BearerConfig = new SwaggerBearerAuthConfig()
    {
        AuthEnabled = true,
        HeaderKey = "Bearer" // the header key that'll be used in format [{HeaderKey} JWT]
    };
    
    config.AddHeader("user-agent", "test agent"); // Adds a header value for all the request

    // To enable for SwaggerUI to show XmlDocumentation you've created for your controllers/actions
    config.XmlDocConfig = new SwaggerDocConfig()
    {
        // in .csproj you must add "<DocumentationFile>TechBuddy.Test.API.xml</DocumentationFile>" first so it generates the documentation file for swagger to use.
        XmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TechBuddy.Test.API.xml")
    };

    // In SwaggerUI, all the endpoints returns OK as default, unless you specify differently by using [ProducesResponseTypeAttribute]
    // I'll mention this property in a different section below
    config.ResponseTypeModelProviderConfig = ResponseTypeModelProviderConfig.CreateDefault();
});
```


After adding Swagger Service in our WebAPI, we have to tell .NET to use it. See followings;

**PS: This part is only for `WebAPI` projects. It's not required for `HttpTrigger Azure Functions`**


With this usage, we are using Swagger as we configured already.

```csharp
app.UseTechBuddySwagger();
```

However, if we also injected the ApiVersioning in the system, we should use Swagger Extension with ApiVersioning support.

```csharp

// To give swagger the apiversion details, we are providing `IApiVersionDescriptionProvider` here
app.UseTechBuddySwaggerWithApiVersioning(app.Services.GetService<IApiVersionDescriptionProvider>());

```

<br>


`ResponseTypeModelProviderConfig` is to provide the required information of any endpoint returns status code and/or model. With default config, Swagger shows that any endpoint will return 200 (OK) HttpStatusCode.
However, you are able to customize that behavior by using `[ProducesResponseTypeAttribute]` on the top of your Action methods. With this attribute not only you provide the response HttpStatusCode but also the response model.
Our Swagger Extension method has also the same feature with some additional config. 

PS: `Pre-Defined HttpMethods` are * Get, Post, Put, Patch, Head, Delete

What can you do to all your endpoints?
 - You can add default `HttpStatusCode` for all endpoints and for all Pre-Defined HttpMethods -> `AddDefaultResponseHttpStatusCodeForAll()` (So HttpStatusCode.OK for `Pre-Defined HttpMethods`)
 - You can add default `HttpStatusCode` for all endpoints with specific `HttpMethod`. 
   - For Example; All POST methods must have HttpStatusCode.Created
   - `AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Post, HttpStatusCode.Created)`
 - You can add default response model for any HttpStatusCode
   - For Example; When the `Httpstatuscode` is `BadRequest`, `ValidationErrorResponseModel` must be returned
   - `AddSpecificTypeForSpecificHttpStatusCode(typeof(ValidationErrorResponseModel), HttpStatusCode.BadRequest)`
   - Note: If you already added BadRequest with `AddDefaultResponseHttpStatusCode()` or `AddDefaultResponseHttpStatusCodeForHttpMethods()`, you cannot use it here too. The HttpStatusCode can either be Default or specified
 - You can exclude any controller from this context which means none of the actions under this controller will `ProduceReturnType`
 - You can exclude any action method from this context which means this action will not `ProduceReturnType`

Let's say you have an endpoint like;

```csharp
[HttpGet]
public Task<bool> IsTrue()
{
    return Task.FromResult(true);
}
```

In this case, you can tell to extension NOT to provide any response type or HttpStatusCode it is because your action is aldready returning back a value (boolean).

When you use `AddDefaultType()` function to tell what types you are assuming that will be default type to provide a response type. 
So in our case, if any action returning Task, IActionResult or ActionResult will have a DefaultStatusCodes or whatever you configured it.


```csharp
builder.Services.ConfigureTechBuddySwagger(config =>
{
    // if constructor parameters are true, it'll automatically generate the default types as well as default HttpStatusCodes
    // If you don't want to have default values, you can call Clear methods.
    var providerConfig = new ResponseTypeModelProviderConfig(generateDefaultTypes: true, generateDefaultStatusCodes: true); // Generate defaults

    providerConfig.ClearDefaultTypes(); // Clear default types
    providerConfig.ClearDefaultResponseHttpStatusCodes(); // Clear default HttpStatusCode

    /*
        DefaultTypes           -> Task, IActionResult, ActionResult
        DefaultHttpStatusCodes -> OK, BadRequest, InternalServerError
    */

    providerConfig
                .AddDefaultType(typeof(Task), typeof(IActionResult), typeof(ActionResult))
                //.AddDefaultResponseHttpStatusCode(HttpStatusCode.OK)
                .AddDefaultResponseHttpStatusCode(HttpStatusCode.InternalServerError)
                .AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Get, HttpStatusCode.OK)
                .AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Post, HttpStatusCode.OK)
                .AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Post, HttpStatusCode.BadRequest)
                .AddSpecificTypeForSpecificHttpStatusCode(typeof(string), HttpStatusCode.OK)
                .AddSpecificTypeForSpecificHttpStatusCode(typeof(ValidationErrorResponseModel), HttpStatusCode.BadRequest)
                .ExcludeAction(nameof(TestController), nameof(TestController.GetExcluded))
                .ExcludeController(nameof(TestController))
                .ExcludeAction(nameof(TestController.GetExcluded))
    ;

    config.ResponseTypeModelProviderConfig = providerConfig;
});
```