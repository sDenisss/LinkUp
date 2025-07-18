using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Persistence;
using Microsoft.EntityFrameworkCore; // Ваш DbContext

namespace LinkUp.Persistence;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        // Здесь важно, что Email - это Value Object.
        // EF Core может автоматически маппить Value Objects,
        // но иногда требуется явно указать, как сравнивать.
        // Если Email.Value является свойством, то прямое сравнение работает.
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user); // EF Core отслеживает изменения
        await _context.SaveChangesAsync();
    }
}
