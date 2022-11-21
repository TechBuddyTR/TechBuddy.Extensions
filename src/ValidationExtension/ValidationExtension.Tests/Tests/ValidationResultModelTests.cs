using TechBuddy.Extension.Validation.Infrastructure.Models;

namespace ValidationExtension.Tests.Tests;
public sealed class ValidationResultModelTests
{
    [Test]
    public void ValidationResultModel_IsValid_ShouldBeFalseAsDefault()
    {
        // Arrange
        var model = new ValidationResultModel();


        // Assert
        model.IsValid.Should().BeFalse();
    }

    [Test]
    public void ValidationResultModel_ErrorMessages_ShouldBeNullAsDefault()
    {
        // Arrange
        var model = new ValidationResultModel();


        // Assert
        model.Errors.Should().BeNull();
    }

    [Test]
    public void ValidationResultModel_Model_ShouldBeNullAsDefault()
    {
        // Arrange
        var model = new ValidationResultModel<TestModel>();


        // Assert
        model.Model.Should().BeNull();
    }
}
