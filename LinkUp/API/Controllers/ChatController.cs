using MediatR;
using Microsoft.AspNetCore.Authorization; // Обязательно для [Authorize]
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Для ClaimTypes
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinkUp.Application;

namespace LinkUp.API;

[ApiController]
[Route("api/[controller]")]
[Authorize] // <--- Этот атрибут ОБЯЗАТЕЛЕН
public class ChatController : ControllerBase
{
    private readonly ISender _sender;

    public ChatController(ISender sender)
    {
        _sender = sender;
    }

    // Маршрут изменен, чтобы не включать userId в URL
    // Вместо этого используйте "my-chats" или что-то подобное.
    [HttpGet("my-chats")] // <--- ОБНОВЛЕННЫЙ МАРШРУТ
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetMyChats(CancellationToken cancellationToken)
    {
        // 1. Получаем ID пользователя из JWT-токена, который уже был проверен
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        // 2. Проверяем, что клейм существует и это валидный GUID
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            // Если этот код выполняется, значит, что-то пошло не так с настройкой JWT
            // или с самим токеном, несмотря на [Authorize].
            // Обычно, если [Authorize] не сработал, сюда бы запрос не дошел.
            return Unauthorized("Не удалось получить идентификатор пользователя из токена. Убедитесь, что токен корректен и содержит 'sub' клейм.");
        }

        // 3. Создаем запрос с полученным ID пользователя
        var query = new GetAllChatsQuery(userId);

        // 4. Отправляем запрос через MediatR
        var chats = await _sender.Send(query, cancellationToken);

        // 5. Обрабатываем результат
        if (chats == null || !chats.Any())
        {
            return NotFound($"Чаты для аутентифицированного пользователя не найдены.");
        }

        return Ok(chats);
    }

    [HttpGet("{chatId}/users")] 
    public async Task<ActionResult<IEnumerable<UserDto>>> GetChatUsers(Guid chatId, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid currentUserId))
        {
            return Unauthorized("Не удалось получить идентификатор пользователя из токена.");
        }

        var userChatsQuery = new GetAllChatsQuery(currentUserId);
        var userChats = await _sender.Send(userChatsQuery, cancellationToken);
        
        if (userChats == null || !userChats.Any(c => c.Id == chatId))
        {
            return Forbid("У вас нет доступа к этому чату.");
        }

        var query = new GetAllUsersInChatQuery(chatId);

        var users = await _sender.Send(query, cancellationToken);

        if (users == null || !users.Any())
        {
            return Ok(Enumerable.Empty<MessageDto>()); 
        }

        return Ok(users);
    }
    
}