
using LinkUp.API;
using LinkUp.Domain;
using LinkUp.Persistence;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LinkUp.Application;
public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Guid>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IHubContext<ChatHub> _hubContext;

    public SendMessageCommandHandler(IMessageRepository messageRepository, IHubContext<ChatHub> hubContext)
    {
        _messageRepository = messageRepository;
        _hubContext = hubContext;
    }

    public async Task<Guid> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message(
            request.Id,
            request.ChatId,
            request.SenderId,
            request.Content,
            DateTime.UtcNow
        );

        await _messageRepository.AddAsync(message, cancellationToken);

         // Оповестить всех участников чата через SignalR
        await _hubContext.Clients.Group(request.ChatId.ToString())
            .SendAsync("ReceiveMessage", new
            {
                message.Id,
                message.SenderId,
                message.Content,
                message.Timestamp
            });

        return message.Id;
    }
}
