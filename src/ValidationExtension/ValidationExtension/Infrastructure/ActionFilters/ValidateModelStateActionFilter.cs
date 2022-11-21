using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TechBuddy.Extension.Validation.Infrastructure.Models.ModelProviders;

namespace TechBuddy.Extension.Validation.Infrastructure.ActionFilters;

/// <summary>
/// The an ActionFilter for NET which handles InValid ModelState and returns the response by calling <see cref="IDefaultModelProvider.GetModel(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary.ValueEnumerable)"/>
/// </summary>
public class ValidateModelStateActionFilter : IAsyncActionFilter
{
    private readonly IDefaultModelProvider defaultModelProvider;

    /// <summary>
    /// Initiates the <see cref="ValidateModelStateActionFilter"/>
    /// </summary>
    /// <param name="defaultModelProvider">The model provider</param>
    public ValidateModelStateActionFilter(IDefaultModelProvider defaultModelProvider)
    {
        this.defaultModelProvider = defaultModelProvider;
    }

    /// <inheritdoc/>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid)
        {
            await next();
            return;
        }

        var model = defaultModelProvider.GetModel(context.ModelState.Values);

        context.Result = new BadRequestObjectResult(model);
    }
}
