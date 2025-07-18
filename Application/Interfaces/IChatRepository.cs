// IChatRepository.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using LinkUp.Domain;

namespace LinkUp.Application;

public interface IChatRepository
{
    // Метод для получения чата по ID, возможно, с eager loading пользователей
    Task<Chat?> GetByIdWithUsersAsync(Guid chatId, CancellationToken cancellationToken);
    Task AddAsync(Chat chat, CancellationToken cancellationToken);
    Task UpdateAsync(Chat chat, CancellationToken cancellationToken); // Метод для сохранения изменений
}
