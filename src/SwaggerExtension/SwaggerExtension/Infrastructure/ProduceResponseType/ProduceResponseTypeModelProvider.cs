using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace TechBuddy.Extensions.OpenApi.Infrastructure.ProduceResponseType;
internal class ProduceResponseTypeModelProvider : IApplicationModelProvider
{
    private readonly ResponseTypeModelProviderConfig config;

    public ProduceResponseTypeModelProvider(ResponseTypeModelProviderConfig config)
    {
        this.config = config;
    }

    public int Order => 1;

    public void OnProvidersExecuted(ApplicationModelProviderContext context) { }

    // This functions tends to add default ResponseType attribute for the actions that doesn't have
    // 200 => OK and 400 => BadRequest (in case of validation error)
    public void OnProvidersExecuting(ApplicationModelProviderContext context)
    {
        var controllers = GetTrimmedControllers(context);

        foreach (ControllerModel controller in controllers)
        {
            IEnumerable<(ActionModel actionModel, IEnumerable<string> httpMethods)> actions
                = GetTrimmedActions(controller);

            foreach ((ActionModel actionModel, IEnumerable<string> httpMethods) actionsWithMethods in actions)
            {
                AddFiltersToAction(actionsWithMethods.actionModel, actionsWithMethods.httpMethods);
            }
        }
    }




    #region Private Methods


    private void AddFiltersToAction(ActionModel action, IEnumerable<string> httpMethods)
    {
        // Add Default Status Code
        foreach (var defaultStatusCode in config.DefaultStatusCodes)
        {
            var eligible = httpMethods.Any(i => i == defaultStatusCode.HttpMethod.ToString());
            if (!eligible)
                continue;

            Type returnType = GetActionReturnModelType(action);

            // If DefaultType
            if (config.DefaultTypes.Contains(returnType))
            {
                var typeBaseStatusCodeFound = config.TypeBaseStatusCodes.Any(i => i.Key == defaultStatusCode.HttpStatusCode);

                if (typeBaseStatusCodeFound)
                    returnType = config.TypeBaseStatusCodes[defaultStatusCode.HttpStatusCode];
            }

            AddFilterToActionModel(action,
                    new ProducesResponseTypeAttribute(returnType, (int)defaultStatusCode.HttpStatusCode));
        }


    }

    /// <summary>
    /// Exclude the provided controllers. And also trims "controller" key in the name of the Controllers
    /// </summary>
    /// <param name="context">The ApplicationModelProviderContext param provided by .NET</param>
    /// <returns>The list of Controllers</returns>
    private IEnumerable<ControllerModel> GetTrimmedControllers(ApplicationModelProviderContext context)
    {
        const string controllerKey = "controller";
        var result = new List<ControllerModel>();

        var trimmedExcludedList = config.ExcludedControllers.Select(i => i.Replace(controllerKey, "", StringComparison.OrdinalIgnoreCase));

        foreach (var controller in context.Result.Controllers)
        {
            var controllerName = controller.ControllerName.Replace(controllerKey, "", StringComparison.OrdinalIgnoreCase);

            var exclude = trimmedExcludedList.Contains(controllerName);

            if (!exclude)
                result.Add(controller);
        }

        return result;
    }

    private IEnumerable<(ActionModel actionModel, IEnumerable<string> httpMethods)> GetTrimmedActions(ControllerModel controller)
    {
        List<(ActionModel actionModel, IEnumerable<string> httpMethods)> result = new();

        foreach (ActionModel action in controller.Actions)
        {
            var notEligible = IfExcludedAction(action);
            if (notEligible)
                continue;

            var httpMethods = action.Attributes
                                    .OfType<HttpMethodAttribute>()
                                    .SelectMany(x => x.HttpMethods).Distinct();

            notEligible = config.DefaultStatusCodes.Any(i => httpMethods.Contains(i.ToString()));

            if (notEligible)
                continue;

            result.Add((action, httpMethods));
        }

        return result;
    }

    private bool IfExcludedAction(ActionModel actionModel)
    {
        // Generate possible action names to check if excluded
        var possibleActionNames = new[]
                                  {
                                     actionModel.ActionName,
                                     $"{actionModel.Controller.ControllerName}.{actionModel.ActionName}",
                                     $"{actionModel.Controller.ControllerName}Controller.{actionModel.ActionName}",
                                  };

        var excluded = config.ExcludedActions.Any(i => possibleActionNames.Contains(i));

        return excluded;
    }

    private Type GetActionReturnModelType(ActionModel actionModel)
    {
        Type returnType = actionModel.ActionMethod.ReturnType;

        while (returnType.GetGenericArguments()?.Length > 0)
        {
            returnType = returnType.GetGenericArguments().First();
        }

        return returnType;
    }

    private static void AddFilterToActionModel(ActionModel action,
                                               ProducesResponseTypeAttribute filter)
    {
        bool alreadyAdded = action.Filters
                            .Where(i => i is ProducesResponseTypeAttribute)
                            .Select(i => (ProducesResponseTypeAttribute)i)
                            .Any(i => i.StatusCode == filter.StatusCode);

        if (alreadyAdded) return;

        action.Filters.Add(filter);

        // To say it produces Json
        action.Filters.Add(new ProducesAttribute(SwaggerConstants.DefaultResponseContentType));
    }

    #endregion
}
