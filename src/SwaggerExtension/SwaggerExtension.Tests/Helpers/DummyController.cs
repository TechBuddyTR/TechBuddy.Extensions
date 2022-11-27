using Microsoft.AspNetCore.Mvc;

namespace SwaggerExtension.Tests.Helpers;
public sealed class DummyController : ControllerBase
{
    [HttpGet]
    public void DummyAction()
    {

    }
}


public sealed class DummyController2 : ControllerBase
{
    [HttpGet]
    public void DummyAction2()
    {

    }
}