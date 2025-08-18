using MediatR;
using Microsoft.AspNetCore.Authorization; // Для атрибута [Authorize]
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
// [Authorize] // <--- ОБЯЗАТЕЛЬНО: Контроллер требует аутентификации
public class MessageController : ControllerBase
{
    private readonly ISender _sender;

    public MessageController(ISender sender)
    {
        _sender = sender;
    }

    // Маршрут теперь принимает ChatId как параметр URL
    [HttpGet("{chatId}/messages")] // <--- ОБНОВЛЕННЫЙ МАРШРУТ
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetChatMessages(Guid chatId, CancellationToken cancellationToken)
    {
        // 1. Получаем ID текущего пользователя из JWT токена.
        // Это нужно, чтобы проверить, имеет ли текущий пользователь доступ к запрашиваемому чату.
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid currentUserId))
        {
            return Unauthorized("Не удалось получить идентификатор пользователя из токена.");
        }

        // Guid currentUserId = Guid.Parse("995decbf-81a5-4fd0-b9ec-1bb523299b1f");

        // --- ВАЖНЫЙ ШАГ БЕЗОПАСНОСТИ: ПРОВЕРКА ДОСТУПА К ЧАТУ ---
        // Прежде чем возвращать сообщения, убедитесь, что currentUserId
        // является членом чата с chatId. Иначе любой аутентифицированный пользователь
        // сможет просматривать сообщения любого чата, зная его ID.
        // Для этого понадобится запрос, который возвращает чаты пользователя
        // (например, GetAllChatsQuery или специализированный IsUserInChatQuery)

        // Пример (предполагает, что GetAllChatsQuery возвращает чаты пользователя):
        var userChatsQuery = new GetAllChatsQuery(currentUserId);
        var userChats = await _sender.Send(userChatsQuery, cancellationToken);
        
        if (userChats == null || !userChats.Any(c => c.Id == chatId))
        {
            // Если пользователь не является членом запрашиваемого чата
            return Forbid("У вас нет доступа к этому чату.");
        }
        // --- КОНЕЦ ПРОВЕРКИ БЕЗОПАСНОСТИ ---


        // 2. Создаем запрос для получения всех сообщений для указанного ChatId
        // Используем GetAllMessagesQuery, который должен принимать ChatId
        var query = new GetAllMessagesQuery(chatId);

        // 3. Отправляем запрос через MediatR
        var messages = await _sender.Send(query, cancellationToken);

        // 4. Обрабатываем результат
        if (messages == null || !messages.Any())
        {
            // Возвращаем пустой список, а не NotFound, если чат пуст.
            // Если чат существует, но в нем нет сообщений, это не "не найдено",
            // а просто "нет сообщений".
            return Ok(Enumerable.Empty<MessageDto>()); 
        }

        return Ok(messages);
    }
}