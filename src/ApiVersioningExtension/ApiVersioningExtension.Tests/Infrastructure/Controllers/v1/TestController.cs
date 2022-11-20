namespace ApiVersioningExtension.Tests.Infrastructure.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class TestController : ControllerBase
{
    [HttpGet()]
    public ActionResult Get()
    {
        return Ok("V1");
    }
}
