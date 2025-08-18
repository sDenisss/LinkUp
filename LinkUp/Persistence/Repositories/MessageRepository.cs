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

    // public Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken)
    {
        // Убедитесь, что эта логика правильна
        return await _context.Messages
                             .Where(m => m.ChatId == chatId)
                             .OrderBy(m => m.Timestamp) // Хорошо для упорядочивания сообщений
                             .ToListAsync(cancellationToken);
    }
}
