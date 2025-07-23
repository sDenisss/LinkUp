// ChatRepository.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Persistence;

namespace LinkUp.Persistence;

public class ChatRepository : IChatRepository
{
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Chat>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Option 1: Start from the Users DbSet and include their Chats.
        // This is often the most intuitive way when you want "all X for a given Y".
        var user = await _context.Users
                                    .Include(u => u.Chats) // Eagerly load the 'Chats' navigation property for the user
                                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            // If no user with that ID is found, they can't have any chats.
            return Enumerable.Empty<Chat>();
        }

        // 'user.Chats' is already an ICollection<Chat>, which implements IEnumerable<Chat>.
        // No casting needed, and it contains all chats for this user.
        return user.Chats;
    }
    public async Task<Chat?> GetByIdWithUsersAsync(Guid chatId, CancellationToken cancellationToken)
    {
        // Загружаем чат вместе с его пользователями (связь _usersInChat)
        return await _context.Chats
                                .Include(c => c.Users) // <-- Важно! Загружаем навигационное свойство Users
                                .FirstOrDefaultAsync(c => c.Id == chatId, cancellationToken);
    }

    public async Task AddAsync(Chat chat, CancellationToken cancellationToken)
    {
        _context.Chats.Add(chat);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Chat chat, CancellationToken cancellationToken)
    {
        // EF Core будет отслеживать изменения в сущности и её дочерних коллекциях,
        // если она загружена из контекста.
        // context.Chats.Update(chat); // Обычно не нужно вызывать Update() явно для отслеживаемых сущностей
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsUserInChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
    {
        return await _context.Chats
                                 .Include(c => c.Users) // Eagerly load the Users collection for the chat
                                 .AnyAsync(c => c.Id == chatId && c.Users.Any(u => u.Id == userId), cancellationToken);
    }

}
