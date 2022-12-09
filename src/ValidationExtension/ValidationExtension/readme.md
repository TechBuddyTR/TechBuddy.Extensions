# Validation Extension


Validation Extension is aiming to give you the opportunity to add FluentValidation in your WebAPI or HttpTrigger Azure Functions. 
In this library we have extensions methods for two different object which are `HttpRequest` for Azure Functions and `IServiceCollection` for WebAPI projects.

<br />


## Nuget
| Package Name | Package | Download |
| ------------- | ------------- | ------------- |
| Validation | [![](https://img.shields.io/nuget/v/TechBuddy.Extensions.Validation?style=for-the-badge)](https://www.nuget.org/packages/TechBuddy.Extensions.Validation) | [![](https://img.shields.io/nuget/dt/TechBuddy.Extensions.Validation?style=for-the-badge)](https://www.nuget.org/packages/TechBuddy.Extensions.Validation/) |

### Installation

```bash
PM> NuGet\Install-Package TechBuddy.Extensions.Validation
```
or
```bash
dotnet add package TechBuddy.Extensions.Validation
```

----


## Usages




##### Test Models Definition

```csharp
public sealed class TestModel
{
    public int Id { get; set; }

    public string Name { get; set; }
}
```

```csharp
public sealed class TestValidator : AbstractValidator<TestModel>
{
    public TestValidator()
    {
        RuleFor(i => i.Id).GreaterThan(0).WithMessage("{PropertyName} cannot be zero!");
        RuleFor(i => i.Name).MinimumLength(3).WithMessage("{PropertyName} must be at least {MinLength} character");
    }
}
```

<br>

## HttpRequest Extensinon Usage

HttpRequest extensinon methods could be used in an *Azure Function* to validate the HttpRequest.Body by providing a model and `IValidator<Model>` class that has validation rules.


### Usage in Azure Functions 1

In Startup.cs

```csharp
public override void Configure(IFunctionsHostBuilder builder)
{
    var context = builder.GetContext();

    builder.Services.AddTechBuddyValidator();
}
```

In Function File

```csharp
private readonly IValidator<TestModel> testValidator;

public TestFunction(IValidator<TestModel> testValidator)
{
    this.testValidator = testValidator;
}


[FunctionName("Function1")]
public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] TestModel testModel)
{
    var validationResult = testValidator.Validate(testModel); // FluentValidation Validate

    if (!validationResult.IsValid)
        return new BadRequestObjectResult(validationResult.Errors.Select(i => i.ErrorMessage));

    return new OkObjectResult(testModel.Name);
}
```

<br>

### Usage in Azure Functions 2

```csharp
[FunctionName("Function2")]
    public async Task<IActionResult> RunTest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        var validationResult = await req.ValidateAsync<TestModel, TestValidator>(); // ValidationExtension Validate

        if (!validationResult.IsValid)
            return new BadRequestObjectResult(validationResult.ErrorMessages);

        var testModel = await req.ReadFromJsonAsync<TestModel>();

        return new OkObjectResult(testModel.Name);
    }
```



<br>
<br>

### Usage in UnitTests


```csharp

// Arrange
var model = new TestModel()
{
    Id = 1,
    Name = "Test"
};

var request = HttpRequestBuilder
                .CreateRequest()
                .SetBody(JsonSerializer.Serialize(model))
                .ToRequest();

// Act
var result = await request.ValidateAsync<TestModel, TestValidator>();


// Assert
result.IsValid.Should().BeTrue();
result.Model.Should().BeEquivalentTo(model);
result.ErrorMessages.Should().BeNullOrEmpty();

```


----

## WebAPI Extensinon Usage

In WebAPI, you can either use HttpRequest Extensions to validate your model manually as we do in Azure Functions, or you can inject all your validators into the system.

There are 3 different extension methods for IServiceCollection, where each has extra `ValidationExtensionConfig` argument

```csharp

// It gets the Assembly where TestValidation is in and start scanning for all IValidator<TModel> classes to inject
services.AddTechBuddyValidatorFromAssemblyContaining<TestValidator>();


// It gets the CallingAssembly which is where this method is called from(the WebApi) and start scanning for all IValidator<TModel> classes to inject
services.AddTechBuddyValidator();


// It uses the provided Assembly to start scanning for all IValidator<TModel> classes to inject
var assembly = Assembly.GetExecutingAssembly();
services.AddTechBuddyValidatorFromAssembly(assembly);

```

### Important Config

`ValidationExtensionConfig` can be provided as a parameter while adding the Validation into our system. 
That config has `UseModelProvider` method where you need to pass a class that is derived from `IDefaultModelProvider`. This model will be used to return and object in ResponseBody once a validation error occured. So basically if there is a validation error, by using `IDefaultModelProvider` you take the control and decide what you will return in response body. See the example below.


**PS: BaseValidationErrorResponseModel is already in the Library, so you don't need to create it.**

```csharp
/// <summary>
/// BaseInvalidResponse Model
/// </summary>
public abstract class BaseValidationErrorResponseModel
{
    /// <summary>
    /// List of exceptions occured
    /// </summary>
    public IEnumerable<string> Errors { get; set; }
}
```

```csharp
public class TestResponse : BaseValidationErrorResponseModel
{
    public int HttpStatusCode { get; set; }
}
```

```csharp
public class TestModelProvider : IDefaultModelProvider
{
    public object GetModel(ModelStateDictionary.ValueEnumerable modelStateValues)
    {
        return new TestResponse()
        {
            Errors = modelStateValues.SelectMany(i => i.Errors).Select(i => string.Join(Environment.NewLine, i.ErrorMessage)),
            HttpStatusCode = (int)HttpStatusCode.BadRequest
        };
    }
}
```

In your Startup.cs or Program.cs

```csharp

builder.Services.AddTechBuddyValidator(config => 
{
    config.UseModelProvider<TestModelProvider>();
});

```


So, if you configure your app like that, the result will be like below;

**Request Body**

```json
{
    "id": 0,
    "Name": "Te"
}
```

**Response**

```json
{
    "httpStatusCode": 400,
    "errors": [
        "Id cannot be zero!",
        "Name must be at least 3 character"
    ]
}
```

However, if you used the default(with out ModelProvider) implementation, the response would be like;

```json
{
    "errors": [
        "Id cannot be zero!",
        "Name must be at least 3 character"
    ]
}
```
