using Microsoft.EntityFrameworkCore;
using LinkUp.Domain; // Если Email - это Value Object, который маппится

namespace LinkUp.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация сущностей и их свойств
        // Например, для User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Username).IsUnique(); // Уникальность имени пользователя

            // Маппинг Value Object Email
            entity.OwnsOne(e => e.Email, email =>
            {
                email.Property(e => e.Value)
                        .HasColumnName("Email") // Имя колонки в базе данных
                        .IsRequired()
                        .HasMaxLength(255);
                email.HasIndex(e => e.Value).IsUnique(); // Уникальность email
            });

            entity.Property(e => e.HashedPassword).IsRequired();
            entity.Property(e => e.RegistrationDate).IsRequired();
        });

        // Для Chat
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreateDate).IsRequired();

            // Явно определяем отношение "многие ко многим" и соединительную таблицу
            entity.HasMany(c => c.Users) // Chat имеет много Users
                  .WithMany() // User имеет много Chats (без обратного навигационного свойства в User)
                  .UsingEntity(j => j.ToTable("ChatUser") // <-- Явно указываем имя соединительной таблицы
                      .HasOne(typeof(Chat)) // Отношение к сущности Chat
                          .WithMany()
                          .HasForeignKey("ChatId") // <-- Имя столбца для ChatId
                          .HasConstraintName("FK_ChatUser_Chat_ChatId"), // Опционально: имя внешнего ключа
                      j => j.HasOne(typeof(User)) // Отношение к сущности User
                          .WithMany()
                          .HasForeignKey("UserId") // <-- Имя столбца для UserId
                          .HasConstraintName("FK_ChatUser_User_UserId") // Опционально: имя внешнего ключа
                  );
        });

        // Для Message
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasConversion<string>(); // Хранить enum как строку

            // Настройка отношений:
            // Сообщение относится к одному чату
            entity.HasOne<Chat>()
                    .WithMany() // Если Chat не управляет коллекцией сообщений напрямую
                    .HasForeignKey(m => m.ChatId)
                    .IsRequired();

            // Сообщение отправлено одним пользователем
            entity.HasOne<User>()
                    .WithMany() // Если User не управляет коллекцией сообщений напрямую
                    .HasForeignKey(m => m.SenderId)
                    .IsRequired();
        });
    }
}