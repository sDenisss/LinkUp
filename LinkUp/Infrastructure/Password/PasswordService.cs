
using BCrypt.Net;
using LinkUp.Application; // Установите пакет BCrypt.Net-Next
namespace LinkUp.Infrastructure;
public class PasswordService : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Уровень сложности (Work Factor): чем выше число, тем безопаснее, но дольше
        // Рекомендуемое значение от 10 до 12
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
