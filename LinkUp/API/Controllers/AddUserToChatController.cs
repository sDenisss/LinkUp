using LinkUp.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LinkUp.API;

[ApiController]
[Route("api/[controller]")]
public class AddUserToChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddUserToChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddUserInchat(AddUserToChatCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}