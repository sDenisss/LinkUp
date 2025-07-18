// ChatRepository.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Persistence; // Assuming ApplicationDbContext is here

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
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
    }
}