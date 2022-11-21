using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using TechBuddy.Extension.Validation.Infrastructure.Models.ModelProviders;

public class TestModelProvider : IDefaultModelProvider
{
    public object GetModel(ModelStateDictionary.ValueEnumerable modelStateValues)
    {
        return new TestResponse()
        {
            Errors = modelStateValues.SelectMany(i => i.Errors).Select(i => string.Join(Environment.NewLine, i.ErrorMessage)).ToList(),
            HttpStatusCode = (int)HttpStatusCode.BadRequest
        };
    }
}