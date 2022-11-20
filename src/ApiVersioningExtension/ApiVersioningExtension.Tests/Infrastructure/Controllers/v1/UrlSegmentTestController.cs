using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ApiVersioningExtension.Tests.Infrastructure.Controllers.v1;


[ApiController]
[Route(TestConstants.ControllerRoute)]
[ApiVersion("1.0")]
public sealed class UrlSegmentTestController : ControllerBase
{
    [HttpGet()]
    public ActionResult Get()
    {
        return Ok("V1");
    }
}
