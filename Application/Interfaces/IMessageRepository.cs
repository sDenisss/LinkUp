using LinkUp.Domain;

namespace LinkUp.Application;

public interface IMessageRepository
{
    // Метод для получения чата по ID, возможно, с eager loading пользователей
    Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken cancellationToken);
    Task AddAsync(Message message, CancellationToken cancellationToken);

    // Task UpdateAsync(Chat chat, CancellationToken cancellationToken); // Метод для сохранения изменений
}
