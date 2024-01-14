using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DapperHomeWork.Controllers;

using Interfaces.Repositories;

[Route("api/[controller]")]
[ApiController]
public class LogsController : ControllerBase
{
    private readonly ILoggerRepository _logRepository;
    public LogsController(ILoggerRepository logRepository)
    {
        _logRepository = logRepository;
    }

    [HttpGet("logs")]
    [Authorize(Roles = "admin")]
    public IActionResult GetLogs()
    {
        var logs = _logRepository.GetAllLogs();

        return Ok(logs);
    }

    [HttpDelete]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteLogs()
    {
        if (!_logRepository.Delete())
            return BadRequest();

        return Ok();
    }
}

