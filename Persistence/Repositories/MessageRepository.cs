using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Persistence;
using Microsoft.EntityFrameworkCore; // Ваш DbContext

namespace LinkUp.Persistence;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Message message, CancellationToken cancellationToken)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }

    public Task<Message> GetMessagesByIdAsync(Guid messageId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
