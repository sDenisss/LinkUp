using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Infrastructure;
using LinkUp.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Application;

public class AddUserToChatCommandHandler : IRequestHandler<AddUserToChatCommand, Guid>
{
    private readonly ApplicationDbContext _db;
    private readonly IChatRepository _chat;
    // private readonly IUserRepository _userRepository;
    public AddUserToChatCommandHandler(ApplicationDbContext db, IChatRepository chat)
    {
        _db = db;
        _chat = chat;
        // _userRepository = userRepository;
    }
    public async Task<Guid> Handle(AddUserToChatCommand request, CancellationToken cancellationToken)
    {
        //var chat = await _db.Chats.FindAsync(request.ChatId, cancellationToken);
        var chat = await _db.Chats
                            .Include(c => c.Users) // Загружаем коллекцию Users, связанную с чатом
                            .FirstOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken);

        var addMember = await _db.Users.FindAsync(request.AddUserId, cancellationToken);
        // var chat = new Chat(request.Name, creator);

        if (chat == null)
        {
            throw new ApplicationException($"Chat with ID {request.ChatId} not found."); // Или более специфичное исключение
        }
        if (addMember == null) // <-- Добавьте эту проверку!
        {
            throw new ApplicationException($"User with ID {request.AddUserId} not found.");
        }

        chat.AddMember(addMember);
        await _db.SaveChangesAsync(cancellationToken);

        return chat.Id;
    }
}
