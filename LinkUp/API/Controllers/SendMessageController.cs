using System.Security.Claims;
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
    private readonly ISender _sender;

    public SendMessageController(IMediator mediator, ISender sender)
    {
        _mediator = mediator;
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessageToChat(SendMessageCommand command, CancellationToken cancellationToken)
    {
        //await _sender.Send(cancellationToken);

        // Отправляем команду через MediatR
        await _sender.Send(command, cancellationToken); // Где SendMessageCommand - это ваш MediatR Command
        return Ok();
    }

    //[HttpPost]
    //public async Task<IActionResult> SendMessageToChat(SendMessageCommand command, CancellationToken cancellationToken)
    //{
    //    // Получите ID отправителя из токена, как мы делали для GetMyChats
    //    var senderIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
    //    if (senderIdClaim == null || !Guid.TryParse(senderIdClaim.Value, out Guid senderId))
    //    {
    //        return Unauthorized("Не удалось получить идентификатор отправителя из токена.");
    //    }

    //    // Проверьте, что senderId в команде совпадает с senderId из токена для безопасности
    //    if (command.SenderId != senderId)
    //    {
    //        return Forbid("Попытка отправить сообщение от чужого имени.");
    //    }

    //    // Перед отправкой команды, убедитесь, что senderId действительно является членом command.ChatId
    //    // Это КРИТИЧЕСКИ ВАЖНО для безопасности, как мы обсуждали для GetChatMessages.
    //    // Используйте IMediator для IsUserInChatQuery или подобного.
    //    var userInChatQuery = new IsUserInChatQuery { UserId = senderId, ChatId = command.ChatId };
    //    var isMember = await _sender.Send(userInChatQuery, cancellationToken);
    //    if (!isMember)
    //    {
    //        return Forbid("Вы не являетесь участником этого чата.");
    //    }

    //    // Отправляем команду через MediatR
    //    await _sender.Send(command, cancellationToken); // Где SendMessageCommand - это ваш MediatR Command
    //    return Ok();
    //}
}