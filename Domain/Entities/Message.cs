namespace LinkUp.Domain;
public class Message
{
    // Свойства сущности
    public Guid Id { get; private set; }
    public Guid ChatId { get; private set; } // К какому чату относится сообщение
    public Guid SenderId { get; private set; } // Кто отправил сообщение
    public string Content { get; private set; } // Содержимое сообщения
    public DateTime Timestamp { get; private set; } // Время отправки
    public MessageStatus Status { get; private set; } // Статус сообщения (отправлено, прочитано и т.д.)

    // Приватный конструктор для EF Core или других ORM,
    // чтобы предотвратить создание объекта в невалидном состоянии извне.
    // Если используете EF Core 6+, можно убрать set; и использовать поля,
    // но с private set; тоже будет работать.
    private Message() { }

    // Публичный конструктор для создания новых сообщений в доменной модели
    // Здесь происходит вся начальная валидация
    public Message(Guid id, Guid chatId, Guid senderId, string content, DateTime timestamp)
    {
        // 1. Валидация входных данных
        if (id == Guid.Empty)
        {
            throw new DomainException("Message ID cannot be empty.");
        }
        if (chatId == Guid.Empty)
        {
            throw new DomainException("Chat ID cannot be empty for a message.");
        }
        if (senderId == Guid.Empty)
        {
            throw new DomainException("Sender ID cannot be empty for a message.");
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainException("Message content cannot be empty.");
        }
        // Можно добавить дополнительные правила, например, максимальную длину контента
        if (content.Length > 1000)
        {
            throw new DomainException("Message content exceeds maximum allowed length.");
        }
        if (timestamp == default) // Проверяем, что дата не является значением по умолчанию
        {
            throw new DomainException("Message timestamp cannot be default.");
        }
        if (timestamp > DateTime.UtcNow.AddMinutes(1)) // Если сообщение не может быть из будущего
        {
            throw new DomainException("Message timestamp cannot be in the future.");
        }

        // 2. Инициализация свойств после успешной валидации
        Id = id;
        ChatId = chatId;
        SenderId = senderId;
        Content = content;
        Timestamp = timestamp;
        Status = MessageStatus.Sent; // Инициализируем начальный статус
    }

    // --- Поведение (методы, инкапсулирующие бизнес-логику) ---

    // Метод для пометки сообщения как прочитанного
    public void MarkAsRead()
    {
        if (Status == MessageStatus.Read)
        {
            // Можно просто выйти или бросить исключение, если это строгое бизнес-правило
            // throw new DomainException("Message is already marked as read.");
            return;
        }
        Status = MessageStatus.Read;
        // Здесь можно было бы создать и опубликовать доменное событие MessageReadEvent
    }

    // Метод для редактирования сообщения (если разрешено бизнес-логикой)
    public void EditContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
        {
            throw new DomainException("Message content cannot be empty after editing.");
        }
        // Пример бизнес-правила: нельзя редактировать сообщение после определенного времени
        // if (DateTime.UtcNow - Timestamp > TimeSpan.FromMinutes(5))
        // {
        //     throw new DomainException("Message can only be edited within 5 minutes of sending.");
        // }
        // Пример бизнес-правила: нельзя редактировать прочитанные сообщения
        // if (Status == MessageStatus.Read)
        // {
        //     throw new DomainException("Read messages cannot be edited.");
        // }

        Content = newContent;
        // Здесь можно было бы создать и опубликовать доменное событие MessageEditedEvent
    }

    // Метод для пометки сообщения как удаленного (мягкое удаление)
    public void MarkAsDeleted()
    {
        if (Status == MessageStatus.Deleted)
        {
            return; // Уже удалено
        }
        Status = MessageStatus.Deleted;
        // Здесь можно было бы создать и опубликовать доменное событие MessageDeletedEvent
    }
}