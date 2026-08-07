using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommandController : ControllerBase
{
    [HttpGet]
    public ActionResult Get()
    {
        return Ok($"Hello from {nameof(CommandController)}");
    }
}