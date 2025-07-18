using Microsoft.AspNetCore.SignalR;

namespace LinkUp.API;
public class ChatHub : Hub
{
    public async Task SendMessageToChat(Guid chatId, Guid senderId, string content)
    {
        // Отправляем сообщение всем клиентам в этом чате
        await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", senderId, content, DateTime.UtcNow);
    }

    public override async Task OnConnectedAsync()
    {
        // можно логировать подключение
        await base.OnConnectedAsync();
    }

    public async Task JoinChat(Guid chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public async Task LeaveChat(Guid chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }
}
