using TechBuddy.Extensions.Tests.Common.TestCommon.Constants;

namespace ApiVersioningExtension.Tests.Infrastructure.Controllers.v2;


[ApiController]
[Route(TestConstants.ControllerRoute)]
[ApiVersion("2.0")]
public sealed class UrlSegmentTestController : ControllerBase
{
    [HttpGet()]
    public ActionResult Get()
    {
        return Ok("V2");
    }
}
