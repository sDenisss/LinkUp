// MessengerApp.Domain/Entities/User.cs
namespace LinkUp.Domain;
public class User
{
    public string HashedPassword { get; private set; }
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string UniqueName { get; private set; }
    public Email Email { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();

    private User() { }
    // Конструктор и методы для бизнес-логики
    public User(string username, Email email, string uniqueName)
    {
        Id = Guid.NewGuid();
        Username = username;
        UniqueName = uniqueName;
        Email = email;
        RegistrationDate = DateTime.UtcNow;
    }

    public User(string username, Email email, string hashedPassword, string uniqueName) : this(username, email, uniqueName)
    {
        HashedPassword = hashedPassword;
    }
}