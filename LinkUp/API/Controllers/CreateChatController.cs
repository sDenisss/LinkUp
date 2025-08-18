using LinkUp.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LinkUp.API;

[ApiController]
[Route("api/[controller]")]
public class CreateChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChat(CreateChatCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

}