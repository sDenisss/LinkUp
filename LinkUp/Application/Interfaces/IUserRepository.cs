

using LinkUp.Domain;

namespace LinkUp.Application;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
    Task<User> GetByUniqueNameAsync(string uniqueName, CancellationToken cancellationToken);
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    // Task<User> GetUniqueNameByIdAsync(Guid userId);
    Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken);

    Task<IEnumerable<User>> GetUsersByChatIdAsync(Guid chatId, CancellationToken cancellationToken);
    // Task DeleteAsync(Guid userId); // Не всегда нужно сразу
}
