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
        // 5초 동안 여기서 대기하게 됩니다.
        var result = await _userService.GetFormattedUserNameAsync(id);

        return Ok(new
        {
            Message = "성공",
            UserId = id,
            Data = result
        });
    }
}