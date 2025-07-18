

using LinkUp.Domain;

namespace LinkUp.Application;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    // Task DeleteAsync(Guid userId); // Не всегда нужно сразу
}
