using LinkUp.Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;

namespace LinkUp.API;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("by-unique-name/{uniqueName}")]
    public async Task<IActionResult> GetByUniqueName(string uniqueName)
    {
        if (string.IsNullOrWhiteSpace(uniqueName))
            return BadRequest("UniqueName не должен быть пустым.");

        var query = new GetByUniqueNameQuery(uniqueName);
        var userDto = await _mediator.Send(query);

        if (userDto == null)
            return NotFound($"Пользователь с уникальным именем \"{uniqueName}\" не найден.");

        return Ok(userDto);
    }


    [HttpGet("unique-name-by-id/{userId}")]
    public async Task<IActionResult> GetUniqueNameById(Guid userId)
    {
        if (string.IsNullOrWhiteSpace(userId.ToString()))
            return BadRequest("UniqueName не должен быть пустым.");

        var query = new GetUniqueNameByIdQuery(userId);
        var userDto = await _mediator.Send(query);

        if (userDto == null)
            return NotFound($"Пользователь с id \"{userId}\" не найден.");

        return Ok(userDto.UniqueName);
    }
}