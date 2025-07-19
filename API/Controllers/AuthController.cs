using LinkUp.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LinkUp.API;

[ApiController]
[Route("api/")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { Token = token });
    }
}
