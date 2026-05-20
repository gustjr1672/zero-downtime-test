using Data.Application;
using Microsoft.AspNetCore.Mvc;

namespace ZeroDowntimeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _userService.GetFormattedUserNameAsync(id);

        return Ok(new
        {
            Version = "V1 (Old - 15초 지연 버전)",
            //Version = "V2 (즉시)",
            Message = "성공",
            UserId = id,
            Data = result
        });
    }

    [HttpGet("quick/{id}")]
    public async Task<IActionResult> GetQuick(int id)
    {
        var result = await _userService.GetFormattedUserNameQuick(id);

        return Ok(new
        {
            Version = "V2 (즉시)",
            Message = "성공",
            UserId = id,
            Data = result
        });
    }
}