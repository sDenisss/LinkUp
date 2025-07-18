// MessengerApp.Domain/ValueObjects/Email.cs
// using LinkUp.Domain.Exceptions; // Предполагается, что у вас есть кастомные доменные исключения

namespace LinkUp.Domain;
public record Email // Используем record для лаконичности Value Object
{
    public string Value { get; init; } // Имя свойства, хранящего адрес email

    // Приватный конструктор, чтобы гарантировать создание только через фабричный метод
    public Email(string value)
    {
        Value = value;
    }

    // Фабричный метод или публичный конструктор для создания экземпляра Email
    public static Email From(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            throw new DomainException("Email address cannot be empty.");
        }

        var trimmedEmail = emailAddress.Trim();

        // 1. Простая первичная проверка, можно расширить
        if (trimmedEmail.EndsWith("."))
        {
            throw new DomainException("Email address cannot end with a dot.");
        }

        // 2. Использование System.Net.Mail.MailAddress для комплексной валидации
        try
        {
            var addr = new System.Net.Mail.MailAddress(trimmedEmail);
            // Убеждаемся, что адрес после парсинга совпадает с исходным
            // (может отсечь некорректные символы или форматирование)
            if (addr.Address != trimmedEmail)
            {
                throw new DomainException("Invalid email format.");
            }
        }
        catch (FormatException ex) // Ловим специфическое исключение для некорректного формата
        {
            throw new DomainException("Invalid email format.", ex);
        }
        catch (Exception ex) // Для других неожиданных ошибок валидации
        {
            throw new DomainException("An unexpected error occurred during email validation.", ex);
        }

        return new Email(trimmedEmail);
    }

    // Опционально: неявное преобразование в string
    public static implicit operator string(Email email) => email.Value;

    // Опционально: явное преобразование из string
    public static explicit operator Email(string value) => From(value);

    // Можно добавить другие методы, специфичные для Email, например, для сравнения доменов
    public bool HasSameDomain(Email otherEmail)
    {
        var thisDomain = Value.Substring(Value.LastIndexOf('@') + 1);
        var otherDomain = otherEmail.Value.Substring(otherEmail.Value.LastIndexOf('@') + 1);
        return thisDomain.Equals(otherDomain, StringComparison.OrdinalIgnoreCase);
    }
}