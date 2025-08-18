// MessengerApp.Domain/Entities/Chat.cs
// using LinkUp.Domain; // Предположим, у вас есть этот класс исключений
namespace LinkUp.Domain;
public class Chat
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public DateTime CreateDate { get; private set; }

        // Это свойство для EF Core. Оно должно быть ICollection<T> и иметь private set
        // virtual нужен для Lazy Loading, если вы его используете.
        public virtual ICollection<User> Users { get; private set; } = new List<User>(); // <--- Изменено

        // Производное свойство
        public int CountOfMembers => Users.Count;

        // Приватный конструктор для EF Core
        private Chat() { }

        // Публичный конструктор для создания нового чата
        public Chat(string name, User creator)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Chat name cannot be empty.");
            if (creator == null)
                throw new DomainException("Chat creator cannot be null.");

            Id = Guid.NewGuid();
            Name = name;
            CreateDate = DateTime.UtcNow;

            Users.Add(creator); // Добавляем создателя в коллекцию Users
        }

        // --- Поведение (методы, инкапсулирующие бизнес-логику) ---

        public void ChangeChatName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new DomainException("New chat name cannot be empty.");
            Name = newName;
        }

        // Этот метод теперь работает с public Users
        public void AddMember(User newMember)
        {
            if (newMember == null)
                throw new DomainException("Cannot add a null user as member.");

            if (Users.Any(u => u.Id == newMember.Id))
            {
                throw new DomainException($"User '{newMember.Username}' is already a member of this chat.");
            }

            Users.Add(newMember); // Добавляем в коллекцию Users
        }

        public void RemoveMember(User memberToRemove)
        {
            if (memberToRemove == null)
                throw new DomainException("Cannot remove a null user as member.");

            var existingMember = Users.FirstOrDefault(u => u.Id == memberToRemove.Id);
            if (existingMember == null)
            {
                throw new DomainException($"User '{memberToRemove.Username}' is not a member of this chat.");
            }

            Users.Remove(existingMember);
        }

    // // Метод для создания сообщения внутри чата
    // public Message CreateMessage(Guid senderId, string content)
    // {
    //     if (string.IsNullOrWhiteSpace(content))
    //         throw new DomainException("Message content cannot be empty.");

    //     // Проверка, что отправитель действительно является участником чата
    //     if (!_usersInChat.Any(u => u.Id == senderId))
    //     {
    //         throw new DomainException($"User with ID '{senderId}' is not a member of this chat.");
    //     }

    //     // Логика создания сообщения. Важно: Message - это отдельная сущность.
    //     // Здесь Chat выступает в роли "агрегата", управляя созданием своих дочерних сущностей.
    //     var message = new Message(Guid.NewGuid(), Id, senderId, content, DateTime.UtcNow);
    //     // Можно добавить message в коллекцию внутри Chat, если это часть агрегата
    //     // Или просто вернуть и позволить прикладной службе сохранить его через MessageRepository

    //     return message;
    // }
}