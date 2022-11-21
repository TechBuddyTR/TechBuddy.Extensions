using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechBuddy.Extension.Validation.Infrastructure.ActionFilters;
using TechBuddy.Extension.Validation.Infrastructure.Models.ConfigModels;
using TechBuddy.Extension.Validation.Infrastructure.Models.ModelProviders;

namespace TechBuddy.Extension.Validation.Extensions;

/// <summary>
/// The extension class for IServiceCollection to inject the Validators
/// </summary>
public static class ValidationDependencyInjectionExtensions
{

    /// <summary>
    /// Registers all the classes derived by IValidator (AbstractValidator included) by scanning all the classes in provided Assembly
    /// </summary>
    /// <param name="services">The ServiceCollection</param>
    /// <param name="assembly">The assembly of your API or where Validators are</param>
    /// <returns>retuns ServiceCollection</returns>
    public static IServiceCollection AddTechBuddyValidatorFromAssembly(this IServiceCollection services,
        Assembly assembly)
    {
        ConfigureModelProvider(services);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }


    /// <summary>
    /// Registers all the classes derived by IValidator (AbstractValidator included) by scanning all the classes in provided Assembly
    /// </summary>
    /// <param name="services">The ServiceCollection</param>
    /// <param name="assembly">The assembly of your API or where Validators are</param>
    /// <param name="configAction">The ValidationExtensionConfig</param>
    /// <returns>retuns ServiceCollection</returns>
    public static IServiceCollection AddTechBuddyValidatorFromAssembly(this IServiceCollection services,
        Assembly assembly,
        Action<ValidationExtensionConfig> configAction)
    {
        var config = new ValidationExtensionConfig();
        configAction.Invoke(config);

        ConfigureModelProvider(services, config);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }


    /// <summary>
    /// Registers all the classes derived by IValidator (AbstractValidator included) by scanning all the classes in Calling Assembly
    /// </summary>
    /// <param name="services">The ServiceCollection</param>
    /// <returns>retuns ServiceCollection</returns>
    public static IServiceCollection AddTechBuddyValidator(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly(); // Get all assemblies from the project where this method is called

        ConfigureModelProvider(services);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }

    /// <summary>
    /// Registers all the classes derived by IValidator (AbstractValidator included) by scanning all the classes in Calling Assembly
    /// </summary>
    /// <param name="services">The ServiceCollection</param>
    /// <param name="configAction">The ValidationExtensionConfig</param>
    /// <returns>retuns ServiceCollection</returns>
    public static IServiceCollection AddTechBuddyValidator(this IServiceCollection services,
                                                           Action<ValidationExtensionConfig> configAction)
    {
        var config = new ValidationExtensionConfig();
        configAction(config); // Fill the config

        ConfigureModelProvider(services, config);

        var assembly = Assembly.GetCallingAssembly(); // Get all assemblies from the project where this method is called

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }



    /// <summary>
    /// Registers all the classes derived by IValidator (AbstractValidator included) by scanning all the classes in the Assembly where <typeparamref name="T"/> is
    /// </summary>
    /// <typeparam name="T">The type of Validator</typeparam>
    /// <param name="services">The ServiceCollection</param>
    /// <returns>retuns ServiceCollection</returns>
    public static IServiceCollection AddTechBuddyValidatorFromAssemblyContaining<T>(this IServiceCollection services)
        where T : IValidator
    {
        ConfigureModelProvider(services);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<T>();

        return services;
    }


    /// <summary>
    /// Registers all the classes derived by IValidator (AbstractValidator included) by scanning all the classes in the Assembly where <typeparamref name="T"/> is
    /// </summary>
    /// <typeparam name="T">The type of Validator</typeparam>
    /// <param name="services">The ServiceCollection</param>
    /// <param name="configAction">The ValidationExtensionConfig</param>
    /// <returns>retuns ServiceCollection</returns>
    public static IServiceCollection AddTechBuddyValidatorFromAssemblyContaining<T>(this IServiceCollection services,
                                                                                    Action<ValidationExtensionConfig> configAction)
        where T : IValidator
    {
        var config = new ValidationExtensionConfig();
        configAction(config);

        ConfigureModelProvider(services, config);

        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<T>();

        return services;
    }



    private static void ConfigureModelProvider(IServiceCollection services, ValidationExtensionConfig config = null)
    {
        services.Configure<ApiBehaviorOptions>(config =>
        {
            config.SuppressModelStateInvalidFilter = true;
        });

        services.AddControllers(config =>
        {
            config.Filters.Add<ValidateModelStateActionFilter>();
        });

        if (config is not null)
        {
            services.AddSingleton(config);
        }

        IDefaultModelProvider modelProvider = config?.ModelProvider ?? new DefaultModelProvider();
        services.AddTransient(i => modelProvider);
    }

}
