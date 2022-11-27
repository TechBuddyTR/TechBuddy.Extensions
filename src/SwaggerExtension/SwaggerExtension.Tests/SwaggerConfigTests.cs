using FluentAssertions;
using TechBuddy.Extensions.OpenApi;
using NUnit.Framework;

namespace SwaggerExtension.Tests;
public sealed class SwaggerConfigTests
{

    #region SubConfigs Null Check Tests

    [Test]
    public void WhenCreated_BeararTokenConfig_ShouldBeNull()
    {
        // Arrange
        var config = new SwaggerConfig();

        // Assert

        config.BearerConfig.Should().BeNull();
    }

    [Test]
    public void WhenCreated_SwaggerDocConfig_ShouldBeNull()
    {
        // Arrange
        var config = new SwaggerConfig();


        // Assert
        config.XmlDocConfig.Should().BeNull();
    }

    [Test]
    public void WhenCreated_ResponseTypeModelProviderConfig_ShouldBeNull()
    {
        // Arrange
        var config = new SwaggerConfig();


        // Assert
        config.ResponseTypeModelProviderConfig.Should().BeNull();
    }

    #endregion

    #region AddHeader Tests

    [Test]
    public void AddHeader_ShouldAddToHeaders()
    {
        // Arrange
        var config = new SwaggerConfig();
        var key = "key";
        var value = "value";

        // Action
        config.AddHeader(key, value);

        // Assert
        config.Headers.Should().ContainKey(key);
        config.Headers.Should().ContainValue(value);
    }

    [Test]
    public void AddHeader_WhenCalledTwice_ShouldReturnArgumentException()
    {
        // Arrange
        var config = new SwaggerConfig();
        var key = "key";
        var value = "value";

        // Action
        config.AddHeader(key, value);

        Action secondAddHeaderAction = () => config.AddHeader(key, value);

        // Assert
        secondAddHeaderAction.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddHeader_WithNullKey_ShouldReturnArgumentNullException()
    {
        // Arrange
        var config = new SwaggerConfig();
        string key = null;
        var value = "value";

        // Action
        Action secondAddHeaderAction = () => config.AddHeader(key, value);

        // Assert
        secondAddHeaderAction.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AddHeader_WithEmptyKey_ShouldReturnArgumentNullException()
    {
        // Arrange
        var config = new SwaggerConfig();
        var key = "";
        var value = "value";

        // Action
        Action secondAddHeaderAction = () => config.AddHeader(key, value);

        // Assert
        secondAddHeaderAction.Should().Throw<ArgumentNullException>();
    }



    [Test]
    public void AddHeaderWithAction_ShouldAddToHeaders()
    {
        // Arrange
        var config = new SwaggerConfig();
        var key = "key";
        var value = "value";

        // Action
        config.AddHeader(key, () => value);

        // Assert
        config.Headers.Should().ContainKey(key);
        config.Headers.Should().ContainValue(value);
    }

    [Test]
    public void AddHeaderWithAction_WhenCalledTwice_ShouldReturnArgumentException()
    {
        // Arrange
        var config = new SwaggerConfig();
        var key = "key";
        var value = "value";

        // Action
        config.AddHeader(key, value);

        Action secondAddHeaderAction = () => config.AddHeader(key, value);

        // Assert
        secondAddHeaderAction.Should().Throw<ArgumentException>();
    }

    [Test]
    public void AddHeaderWithAction_WithNullKey_ShouldReturnArgumentNullException()
    {
        // Arrange
        var config = new SwaggerConfig();
        string key = null;
        var value = "value";

        // Action
        Action secondAddHeaderAction = () => config.AddHeader(key, value);

        // Assert
        secondAddHeaderAction.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void AddHeaderWithAction_WithEmptyKey_ShouldReturnArgumentNullException()
    {
        // Arrange
        var config = new SwaggerConfig();
        var key = "";

        // Action
        Action secondAddHeaderAction = () => config.AddHeader(key, () => "");

        // Assert
        secondAddHeaderAction.Should().Throw<ArgumentNullException>();
    }

    #endregion
}
