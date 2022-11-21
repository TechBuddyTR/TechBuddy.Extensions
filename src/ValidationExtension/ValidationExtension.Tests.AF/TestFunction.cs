using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Linq;
using System.Threading.Tasks;
using TechBuddy.Extension.Validation.Extensions;
using ValidationExtension.Tests.AF.Infrastructure.Helpers.Validators;
using ValidationExtension.Tests.AF.Infrastructure.Models;

namespace ValidationExtension.Tests.AF;

public class TestFunction
{
    private readonly IValidator<TestModel> testValidator;

    public TestFunction(IValidator<TestModel> testValidator)
    {
        this.testValidator = testValidator;
    }


    [FunctionName("Function1")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] TestModel testModel)
    {
        var validationResult = testValidator.Validate(testModel); // FluentValidation Validate

        if (!validationResult.IsValid)
            return new BadRequestObjectResult(validationResult.Errors.Select(i => i.ErrorMessage));

        return new OkObjectResult(testModel.Name);
    }


    [FunctionName("Function2")]
    public async Task<IActionResult> RunTest(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        var validationResult = await req.ValidateAsync<TestModel, TestValidator>(); // ValidationExtension Validate

        if (!validationResult.IsValid)
            return new BadRequestObjectResult(validationResult.Errors);
        
        var testModel = await req.ReadFromJsonAsync<TestModel>();
        //var testModel = validationResult.Model;

        return new OkObjectResult(testModel.Name);
    }
}
