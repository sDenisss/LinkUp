// ChatHub.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using MediatR;
using LinkUp.Application; // Если MediatR нужен в хабе

[Authorize] // <--- Хаб требует JWT токен для подключения
public class ChatHub : Hub
{
    private readonly ISender _sender;
    private readonly IUserRepository _userRepository; // Если хотите получать имя пользователя

    public ChatHub(ISender sender, IUserRepository userRepository)
    {
        _sender = sender;
        _userRepository = userRepository;
    }

    // In your ChatHub.cs
    public override async Task OnConnectedAsync()
    {
        try
        {
            // Log connection ID
            Console.WriteLine($"Client connected: {Context.ConnectionId}");

            // Optionally, try to get user ID, but be prepared for errors
            var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                Console.WriteLine($"User {userIdClaim.Value} connected.");
            }
            else
            {
                Console.WriteLine("Connected client does not have a NameIdentifier claim.");
            }
        }
        catch (Exception ex)
        {
            // THIS IS CRUCIAL: Log any exceptions that occur during connection
            Console.Error.WriteLine($"Error in OnConnectedAsync: {ex}");
            // You might even throw a HubException here to send a more specific error to the client
            throw new HubException("An error occurred during connection setup.");
        }
        await base.OnConnectedAsync();
    }
    // In your ChatHub.cs -> JoinChat method
    public async Task JoinChat(Guid chatId)
    {
        Guid userId;
        try
        {
            userId = GetCurrentUserId(); // Check if this throws
        }
        catch (HubException ex)
        {
            Console.Error.WriteLine($"Error getting current user ID in JoinChat: {ex.Message}");
            throw; // Re-throw to client
        }

        Console.WriteLine($"User {userId} attempting to join chat {chatId}");

        try
        {
            var isMemberQuery = new IsUserInChatQuery { UserId = userId, ChatId = chatId };
            var isMember = await _sender.Send(isMemberQuery);

            if (!isMember)
            {
                Console.Error.WriteLine($"User {userId} is NOT authorized to join chat {chatId}. Forbidding connection.");
                throw new HubException("You do not have access to this chat."); // This is likely it!
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
            Console.WriteLine($"User {userId} successfully joined chat {chatId}.");
            await Clients.Caller.SendAsync("ChatJoined", chatId);
        }
        catch (HubException) // Catch HubExceptions thrown by access checks
        {
            throw; // Re-throw them to the client as specific errors
        }
        catch (Exception ex) // Catch any other unexpected errors
        {
            Console.Error.WriteLine($"Unexpected error in JoinChat for user {userId}, chat {chatId}: {ex}");
            throw new HubException("An unexpected error occurred while joining the chat.", ex); // Provide generic error to client
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            // This indicates a problem with the JWT payload or SignalR's auth
            throw new HubException("User ID claim not found or is invalid in the token.");
        }
        return userId;
    }

    public async Task SendMessageToChat(Guid chatId, Guid senderId, string content)
    {
        var currentUserId = GetCurrentUserId();
        // Проверка, что senderId в сообщении совпадает с ID пользователя из токена
        if (senderId != currentUserId)
        {
            throw new HubException("Attempted to send message on behalf of another user.");
        }

        // *** ОБЯЗАТЕЛЬНО: Проверка доступа пользователя к чату перед отправкой сообщения ***
        var isMemberQuery = new IsUserInChatQuery { UserId = currentUserId, ChatId = chatId };
        var isMember = await _sender.Send(isMemberQuery);
        if (!isMember)
        {
            throw new HubException("You cannot send messages to this chat, you are not a member.");
        }

        // Получаем имя отправителя для отображения на фронтенде
        var sender = await _userRepository.GetByIdAsync(currentUserId); // Метод в IUserRepository
        var senderName = sender?.Username ?? "Unknown User"; // Имя пользователя

        var timestamp = DateTime.UtcNow; // Генерируем время на сервере

        // Отправляем сообщение всем в группе, включая отправителя
        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", chatId, currentUserId, senderName, content, timestamp);
        Console.WriteLine($"Message from {senderName} in {chatId}: {content}");

        // Сохранение в БД можно делать здесь же, если хотите, чтобы SignalR и DB были теснее связаны,
        // или продолжать делать это через отдельный API-эндпоинт, как сейчас.
        // Если делаете здесь, можно использовать команду MediatR
        // await _sender.Send(new SendMessageCommand { ChatId = chatId, SenderId = currentUserId, Content = content });
    }
}

// Пример IsUserInChatQuery (в LinkUp.Application/Features/Chats/Queries)
public class IsUserInChatQuery : IRequest<bool>
{
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }
}
// Пример хендлера для IsUserInChatQuery
public class IsUserInChatQueryHandler : IRequestHandler<IsUserInChatQuery, bool>
{
    private readonly IChatRepository _chatRepository;
    public IsUserInChatQueryHandler(IChatRepository chatRepository)
    {
        _chatRepository = chatRepository;
    }

    public async Task<bool> Handle(IsUserInChatQuery request, CancellationToken cancellationToken)
    {
        // Реализуйте проверку в вашем репозитории чатов
        // Это может быть что-то вроде _chatRepository.IsUserInChatAsync(request.UserId, request.ChatId)
        return await _chatRepository.IsUserInChatAsync(request.UserId, request.ChatId, cancellationToken);
    }
}