using FluentAssertions;
using TechBuddy.Extensions.OpenApi;
using NUnit.Framework;

namespace SwaggerExtension.Tests;
public sealed class SwaggerDocConfigTests
{
    [Test]
    public void XmlDocEnabled_WhenSetFilePath_ShouldBeEnabled()
    {
        // Arrange
        var config = new SwaggerDocConfig
        {
            XmlDocEnabled = false,
            // Action
            XmlFilePath = "C:\\test.xml"
        };

        // Assert
        config.XmlDocEnabled.Should().BeTrue();
    }


    [Test]
    public void XmlFilePath_WhenSetFilePath_ShouldBeSame()
    {
        // Arrange
        var filePath = "C:\\test.xml";


        // Action
        var config = new SwaggerDocConfig
        {
            XmlDocEnabled = false,
            XmlFilePath = filePath
        };

        // Assert
        config.XmlFilePath.Should().Be(filePath);
    }
}
