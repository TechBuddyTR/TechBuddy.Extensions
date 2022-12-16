using Microsoft.AspNetCore.Mvc;

namespace SwaggerExtension.Tests.WebApi.Controllers.v1;

[Route("api/[controller]")]
[ApiController]
[ApiVersion("1")]
public class TestController : ControllerBase
{
    /// <summary>
    /// Gets OK1 result
    /// </summary>
    /// <returns>IActionResult</returns>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("OK1");
    }

    /// <summary>
    /// Gets the FullName property in the post model
    /// </summary>
    /// <param name="testModel"></param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    //[ProducesResponseType(StatusCodes.Status400BadRequest, typeof(BadRequestResponseModel))]
    public ActionResult Post(TestModel testModel)
    {
        if (testModel.Id == 0)
            return BadRequest("Error!");

        return Ok(testModel);
    }
}