using Microsoft.AspNetCore.Mvc;

namespace SwaggerExtension.Tests.WebApi.Controllers.v2;

[Route("api/[controller]")]
[ApiController]
[ApiVersion("2")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("OK2");
    }
}