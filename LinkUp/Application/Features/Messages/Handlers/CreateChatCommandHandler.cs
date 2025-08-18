using LinkUp.Domain;
using LinkUp.Infrastructure;
using LinkUp.Persistence;
using MediatR;

namespace LinkUp.Application;

public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, Guid>
{
    private readonly ApplicationDbContext _db;
    // private readonly IUserRepository _userRepository;

    public CreateChatCommandHandler(ApplicationDbContext db)
    {
        _db = db;
        // _userRepository = userRepository;
    }


    public async Task<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        var creator = await _db.Users.FindAsync(request.CreatorId, cancellationToken);
        if (creator == null)
        {
            throw new ApplicationException($"Creator user with ID {request.CreatorId} not found."); // Или более специфичное исключение
        }
        var chat = new Chat(request.Name, creator);

        _db.Chats.Add(chat);
        await _db.SaveChangesAsync(cancellationToken);

        return chat.Id;
    }
}