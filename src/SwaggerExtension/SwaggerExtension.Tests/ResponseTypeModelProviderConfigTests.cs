using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TechBuddy.Extensions.OpenApi;
using NUnit.Framework;
using SwaggerExtension.Tests.Helpers;
using System.Net;

namespace SwaggerExtension.Tests;
public sealed class ResponseTypeModelProviderConfigTests
{
    #region DefaultType Tests

    [Test]
    public void WhenGenerateDefaultTypeTrue_ShouldGenerate()
    {
        // Arrange
        var generateDefaultTypes = true;
        var config = new ResponseTypeModelProviderConfig(generateDefaultTypes, false);

        // Assert
        config.DefaultTypes.Should().HaveCountGreaterThan(0);
    }

    [Test]
    public void WhenGenerateDefaultTypeFalse_ShouldHaveNoTypes()
    {
        // Arrange
        var generateDefaultTypes = false;
        var config = new ResponseTypeModelProviderConfig(generateDefaultTypes, false);

        // Assert
        config.DefaultTypes.Should().BeEmpty();
    }

    [Test]
    public void AddDefaultType_WithSingleType_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddDefaultType(typeof(IActionResult));

        // Assert
        config.DefaultTypes.Should().NotBeEmpty();
    }

    [Test]
    public void AddDefaultType_WithMultiType_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddDefaultType(typeof(IActionResult));
        config.AddDefaultType(typeof(ActionResult));
        config.AddDefaultType(typeof(Task));

        // Assert
        config.DefaultTypes.Should().NotBeEmpty().And.HaveCount(3);
    }


    [Test]
    public void AddDefaultType_WithDuplicateType_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);
        Action action = () => config.AddDefaultType(typeof(IActionResult));

        // Action
        action(); // first call

        // Assert
        config.DefaultTypes.Should().HaveCount(1);
        action.Should().Throw<ArgumentException>(); // second call
    }


    [Test]
    public void ClearDefaultTypes_ShouldClear()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ClearDefaultTypes();

        // Assert
        config.DefaultTypes.Should().BeEmpty();
    }

    #endregion

    #region DefaultHttpStatusCode Tests

    [Test]
    public void WhenGenerateHttpStatusCodeTrue_ShouldGenerate()
    {
        // Arrange
        var generateDefaultHttpStatusCodes = true;
        var config = new ResponseTypeModelProviderConfig(false, generateDefaultHttpStatusCodes);

        // Assert
        config.DefaultStatusCodes.Should().HaveCountGreaterThan(0);
    }

    [Test]
    public void WhenGenerateHttpStatusCodeFalse_ShouldHaveNoTypes()
    {
        // Arrange
        var generateDefaultHttpStatusCodes = false;
        var config = new ResponseTypeModelProviderConfig(false, generateDefaultHttpStatusCodes);

        // Assert
        config.DefaultStatusCodes.Should().BeEmpty();
    }

    [Test]
    public void AddDefaultHttpStatusCode_WithSingleHttpStatusCode_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddDefaultResponseHttpStatusCodeForAll(HttpStatusCode.OK);

        // Assert
        var count = config.DefaultHttpMethods.Count;

        config.DefaultStatusCodes.Count.Should().Be(count * 1); // one StatusCode added so must be added the code for all HttpMethods
        config.DefaultStatusCodes.Select(i => i.HttpStatusCode).Should().Contain(HttpStatusCode.OK);
    }

    [Test]
    public void AddDefaultHttpStatusCode_WithMultiHttpStatusCode_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddDefaultResponseHttpStatusCodeForAll(HttpStatusCode.OK);
        config.AddDefaultResponseHttpStatusCodeForAll(HttpStatusCode.BadRequest);

        // Assert
        var count = config.DefaultHttpMethods.Count;

        config.DefaultStatusCodes.Count.Should().Be(count * 2); // 2 StatusCodes added so must be added the code for all HttpMethods
        config.DefaultStatusCodes.Select(i => i.HttpStatusCode).Should().Contain(HttpStatusCode.OK);
        config.DefaultStatusCodes.Select(i => i.HttpStatusCode).Should().Contain(HttpStatusCode.BadRequest);
    }

    [Test]
    public void AddDefaultHttpStatusCode_WithDuplicateCode_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);
        Action action = () => config.AddDefaultResponseHttpStatusCodeForAll(HttpStatusCode.OK);

        // Action
        action(); // first call

        // Assert
        action.Should().Throw<ArgumentException>(); // second call
    }


    [Test]
    public void AddDefaultHttpStatusCodeForHttpMethods_WithSingleHttpStatusCode_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Get, HttpStatusCode.OK);

        // Assert
        config.DefaultStatusCodes.Should().NotBeEmpty().And.HaveCount(1);
    }

    [Test]
    public void AddDefaultHttpStatusCodeForHttpMethods_WithMultiHttpStatusCode_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Get, HttpStatusCode.OK);
        config.AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Post, HttpStatusCode.OK);

        // Assert
        config.DefaultStatusCodes.Should().NotBeEmpty().And.HaveCount(2);
    }

    [Test]
    public void AddDefaultHttpStatusCodeForHttpMethods_WithMultiHttpStatusCodeForSameMethod_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Get, HttpStatusCode.OK);
        Action act = () => config.AddDefaultResponseHttpStatusCodeForHttpMethods(HttpMethod.Get, HttpStatusCode.OK);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ClearDefaultHttpStatusCodes_ShouldClear()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ClearDefaultResponseHttpStatusCodes();

        // Assert
        config.DefaultStatusCodes.Should().BeEmpty();
    }

    #endregion

    #region SpecificType Tests

    [Test]
    public void AddSpecificType_WithSingleType_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddSpecificTypeForSpecificHttpStatusCode(HttpStatusCode.OK, typeof(string));

        // Assert

        config.TypeBaseStatusCodes.Should().NotBeEmpty().And.HaveCount(1);
    }


    [Test]
    public void AddSpecificType_WithMultiType_ShouldAdd()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddSpecificTypeForSpecificHttpStatusCode(HttpStatusCode.OK, typeof(string));
        config.AddSpecificTypeForSpecificHttpStatusCode(HttpStatusCode.BadRequest, typeof(int));

        // Assert
        config.TypeBaseStatusCodes.Should().NotBeEmpty().And.HaveCount(2);
    }


    [Test]
    public void AddSpecificType_WithDuplicateType_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.AddSpecificTypeForSpecificHttpStatusCode(HttpStatusCode.OK, typeof(string));
        var action = () => config.AddSpecificTypeForSpecificHttpStatusCode(HttpStatusCode.BadRequest, typeof(string));

        // Assert
        action.Should().Throw<ArgumentException>();
    }


    #endregion

    #region ExcludeController Tests

    [Test]
    public void ExcludeController_WithSingleController_ShouldExclude()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ExcludeController(nameof(DummyController));

        // Assert
        config.ExcludedControllers.Should().HaveCount(1);
    }

    [Test]
    public void ExcludeController_WithMultiController_ShouldExclude()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ExcludeController(nameof(DummyController));
        config.ExcludeController(nameof(DummyController2));

        // Assert
        config.ExcludedControllers.Should().HaveCount(2);
    }


    [Test]
    public void ExcludeController_WithDuplicateController_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ExcludeController(nameof(DummyController)); // first call
        var action = () => config.ExcludeController(nameof(DummyController));

        // Assert
        action.Should().Throw<ArgumentException>();
    }


    [Test]
    public void ExcludeController_WhenControllerNameIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        var action = () => config.ExcludeController(null);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    #endregion

    #region ExcludeAction Tests

    [Test]
    public void ExcludeAction_WithSingleAction_ShouldExclude()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ExcludeAction(nameof(DummyController.DummyAction));

        // Assert
        config.ExcludedActions.Should().HaveCount(1);
    }

    [Test]
    public void ExcludeAction_WithMultiController_ShouldExclude()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ExcludeAction(nameof(DummyController.DummyAction));
        config.ExcludeAction(nameof(DummyController2.DummyAction2));

        // Assert
        config.ExcludedActions.Should().HaveCount(2);
    }


    [Test]
    public void ExcludeAction_WithDuplicateController_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ExcludeAction(nameof(DummyController.DummyAction)); // first call
        var action = () => config.ExcludeAction(nameof(DummyController.DummyAction));

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ExcludeAction_WithDuplicateController_ShouldThrowArgumentException1()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        config.ExcludeAction(nameof(DummyController), nameof(DummyController.DummyAction));

        // Assert
        var name = config.ExcludedActions.First();
        name.Should().Be($"{nameof(DummyController)}.{nameof(DummyController.DummyAction)}");
    }


    [Test]
    public void ExcludeAction_WhenActionNameIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        var action = () => config.ExcludeAction(null);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ExcludeActionWithControllerName_WhenActionNameIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        var action = () => config.ExcludeAction(nameof(DummyController), null);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void ExcludeActionWithControllerName_WhenControllerNameIsNull_ShouldThrowArgumentException()
    {
        // Arrange
        var config = new ResponseTypeModelProviderConfig(false, false);

        // Action
        var action = () => config.ExcludeAction(null, nameof(DummyController.DummyAction));

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    #endregion
}
