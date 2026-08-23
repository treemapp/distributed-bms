using Microsoft.AspNetCore.Mvc;

namespace DistributedBms.Api;

[ApiController]
[Route("api/status")]
public class StatusController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok"
        });
    }
}