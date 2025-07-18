using LinkUp.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LinkUp.API;

[ApiController]
[Route("api/[controller]")]
public class SendMessageController : ControllerBase
{
    private readonly IMediator _mediator;

    public SendMessageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessageToChat(SendMessageCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}