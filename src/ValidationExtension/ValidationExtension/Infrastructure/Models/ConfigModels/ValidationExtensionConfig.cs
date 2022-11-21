using TechBuddy.Extension.Validation.Infrastructure.Models.ModelProviders;

namespace TechBuddy.Extension.Validation.Infrastructure.Models.ConfigModels;
/// <summary>
/// The ValidationExtensionConfig model
/// </summary>
public class ValidationExtensionConfig
{
    internal IDefaultModelProvider ModelProvider { get; set; }


    /// <summary>
    /// Adds an instance of the model that derived from <see cref="IDefaultModelProvider"/>
    /// </summary>
    /// <param name="modelProvider">The model</param>
    public void UseModelProvider<T>(T modelProvider)
        where T : IDefaultModelProvider
    {
        ModelProvider = modelProvider;
    }

    /// <summary>
    /// Sets the model that derived from <see cref="IDefaultModelProvider"/>
    /// </summary>
    /// <typeparam name="T"><see cref="IDefaultModelProvider"/></typeparam>
    public void UseModelProvider<T>()
        where T : IDefaultModelProvider
    {
        ModelProvider = Activator.CreateInstance<T>();
    }

}
