using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechBuddy.Extension.Validation.Extensions;
using TechBuddy.Extension.Validation.Infrastructure.Models.ModelProviders;
using ValidationExtension.Tests.Infrastructure.Helpers.ModelProviders;

namespace ValidationExtension.Tests.Tests;
public sealed class ValidationDIExtensionsTests
{
    #region AddTechBuddyValidatorFromAssemblyContaining Tests

    [Test]
    public void AddValidatorFromAssemblyContaining_ShouldRegisterSpecificValidator()
    {
        // Arrange
        var services = new ServiceCollection();

        // Action
        services.AddTechBuddyValidatorFromAssemblyContaining<TestValidator>();

        var provider = services.BuildServiceProvider();
        var validator = provider.GetService<TestValidator>();

        // Assert
        validator.Should().NotBeNull();
    }

    [Test]
    public void AddValidatorFromAssemblyContaining_ShouldRegisterAllValidators()
    {
        // Arrange
        var services = new ServiceCollection();

        // Action
        services.AddTechBuddyValidatorFromAssemblyContaining<TestValidator>();

        var provider = services.BuildServiceProvider();
        var validator = provider.GetService<TestValidator>();
        var validator2 = provider.GetService<TestValidatorV2>();

        // Assert
        validator.Should().NotBeNull();
        validator2.Should().NotBeNull();
    }

    #endregion

    #region AddTechBuddyValidator Tests

    [Test]
    public void AddValidator_WithNoAssembly_ShouldRegisterSpecificValidator()
    {
        // Arrange
        var services = new ServiceCollection();

        // Action
        services.AddTechBuddyValidator();

        var provider = services.BuildServiceProvider();
        var validator = provider.GetService<TestValidator>();

        // Assert
        validator.Should().NotBeNull();
    }

    [Test]
    public void AddValidator_WithNoAssembly_ShouldRegisterAllValidator()
    {
        // Arrange
        var services = new ServiceCollection();

        // Action
        services.AddTechBuddyValidator();

        var provider = services.BuildServiceProvider();
        var validator = provider.GetService<TestValidator>();
        var validator2 = provider.GetService<TestValidatorV2>();

        // Assert
        validator.Should().NotBeNull();
        validator2.Should().NotBeNull();
    }

    [Test]
    public void AddValidator_WithAssembly_ShouldRegisterSpecificValidator()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Action
        services.AddTechBuddyValidatorFromAssembly(assembly);

        var provider = services.BuildServiceProvider();
        var validator = provider.GetService<TestValidator>();

        // Assert
        validator.Should().NotBeNull();
    }

    [Test]
    public void AddValidator_WithAssembly_ShouldRegisterAllValidator()
    {
        // Arrange
        var services = new ServiceCollection();
        var assembly = Assembly.GetExecutingAssembly();

        // Action
        services.AddTechBuddyValidatorFromAssembly(assembly);

        var provider = services.BuildServiceProvider();
        var validator = provider.GetService<TestValidator>();
        var validator2 = provider.GetService<TestValidatorV2>();

        // Assert
        validator.Should().NotBeNull();
        validator2.Should().NotBeNull();
    }

    [Test]
    public void AddValidatorWithConfig_WithNoAssembly_ShouldRegisterSpecificValidator()
    {
        // Arrange
        var services = new ServiceCollection();

        // Action
        services.AddTechBuddyValidator(conf =>
        {
            conf.UseModelProvider<TestModelProvider>();
        });

        var provider = services.BuildServiceProvider();
        var modelProvider = provider.GetService<IDefaultModelProvider>();

        // Assert
        modelProvider.Should().BeOfType<TestModelProvider>();
    }

    #endregion
}
