using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;
using TechBuddy.Extension.Validation.Extensions;
using TransactionAccess.API;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TransactionAccess.API;

[ExcludeFromCodeCoverage]
public class Startup : FunctionsStartup
{
    /// <summary>
    /// Configures the specified builder.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var context = builder.GetContext();

        builder.Services.AddTechBuddyValidator();
    }

    /// <summary>
    /// Configures the application configuration.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
            .SetBasePath(Environment.CurrentDirectory)
#if DEBUG
            .AddJsonFile("local.settings.json", true, true)
#endif
            .AddEnvironmentVariables();
    }
}
