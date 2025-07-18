// MessengerApp.Application/Features/Users/Commands/Handlers/RegisterUserCommandHandler.cs
using LinkUp.Domain;
using LinkUp.Infrastructure;
using LinkUp.Persistence;
using MediatR;

namespace LinkUp.Application;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    public RegisterUserCommandHandler(ApplicationDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var hashPassword = _passwordHasher.HashPassword(request.Password);
        var user = new User(request.Username, email, hashPassword, request.UniqueName);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    // private string HashPassword(string password)
    // {
    //     return _passwordService.HashPassword(password); // заменишь на реальный хэш позже
    // }
}


// public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IPasswordHasher _passwordHasher;
//     private readonly IEmailService _emailService; // Для отправки приветственного письма

//     public RegisterUserCommandHandler(
//         IUserRepository userRepository,
//         IPasswordHasher passwordHasher,
//         IEmailService emailService)
//     {
//         _userRepository = userRepository;
//         _passwordHasher = passwordHasher;
//         _emailService = emailService;
//     }

//     public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
//     {
//         // 1. Валидация бизнес-правил, требующих обращения к репозиторию
//         // Проверяем уникальность имени пользователя
//         var existingUserByUsername = await _userRepository.GetByUsernameAsync(request.Username);
//         if (existingUserByUsername != null)
//         {
//             throw new ApplicationException($"Username '{request.Username}' is already taken.");
//         }

//         // Проверяем уникальность Email
//         var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
//         if (existingUserByEmail != null)
//         {
//             throw new ApplicationException($"Email '{request.Email}' is already registered.");
//         }

//         // 2. Хешируем пароль
//         var hashedPassword = _passwordHasher.HashPassword(request.Password);

//         // 3. Создаем Value Object Email
//         Email userEmail;
//         try
//         {
//             userEmail = Email.From(request.Email); // Валидация email происходит здесь
//         }
//         catch (DomainException ex)
//         {
//             // Если валидация email в домене не удалась, выбрасываем ApplicationException
//             // или другой подходящий тип исключения для слоя приложения.
//             throw new ApplicationException($"Invalid email format: {ex.Message}", ex);
//         }

//         // 4. Создаем новую доменную сущность User
//         // Вся внутренняя валидация User происходит в его конструкторе
//         var newUser = new User(request.Username, userEmail, hashedPassword);

//         // 5. Сохраняем пользователя через репозиторий
//         await _userRepository.AddAsync(newUser);

//         // 6. Опционально: отправляем приветственное письмо
//         // Это внешний эффект, обрабатывается через инфраструктурный сервис
//         try
//         {
//             await _emailService.SendEmailAsync(newUser.Email, "Welcome to MessengerApp!", $"Hello {newUser.Username},\n\nWelcome aboard!");
//         }
//         catch (Exception ex)
//         {
//             // Логируем ошибку, но не бросаем исключение, чтобы не блокировать регистрацию,
//             // если отправка email не критична для основного сценария.
//             // Возможно, использовать систему очередей для повторной отправки.
//             Console.WriteLine($"Failed to send welcome email to {newUser.Email}: {ex.Message}");
//         }

//         // 7. Возвращаем ID нового пользователя
//         return newUser.Id;
//     }
// }
