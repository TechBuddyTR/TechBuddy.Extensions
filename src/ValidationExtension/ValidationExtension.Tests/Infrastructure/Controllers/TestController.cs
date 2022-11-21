using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace ValidationExtension.Tests.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet()]
    public ActionResult Get()
    {
        return Ok("OK");
    }

    [HttpPost()]
    public ActionResult Post([FromBody] TestModel testModel)
    {
        return Ok(testModel.Name);
    }

    [HttpPost("SkipValidation")]
    public ActionResult PostSkipValidation([CustomizeValidator(Skip = true)][FromBody] TestModel testModel)
    {
        return Ok(testModel.Name);
    }
}