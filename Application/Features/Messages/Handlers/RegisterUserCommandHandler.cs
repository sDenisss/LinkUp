// MessengerApp.Application/Features/Users/Commands/Handlers/RegisterUserCommandHandler.cs
using LinkUp.Domain;
using LinkUp.Infrastructure;
using LinkUp.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Application;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    public RegisterUserCommandHandler(ApplicationDbContext db, IPasswordHasher passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var email = new Email(request.Email);
            var hashPassword = _passwordHasher.HashPassword(request.Password);
            var user = new User(request.Username, email, hashPassword, request.UniqueName);

            var uniqueUserExists = await _db.Users.AnyAsync(u => u.UniqueName == request.UniqueName);
            var emailExists = await _db.Users.AnyAsync(u => u.Email.Value == request.Email);

            if (uniqueUserExists)
            {
                return Result.Failure("Пользователь с таким именем уже существует.");
            }

            if (emailExists)
            {
                throw new ApplicationException("Пользователь с таким именем уже существует.");
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate key") == true)
        {

            throw new ApplicationException("Пользователь с таким именем или почтой уже существует.");
        }
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
