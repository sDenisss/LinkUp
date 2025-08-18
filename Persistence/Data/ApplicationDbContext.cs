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

        // ... (User configuration remains the same) ...
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            // entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.UniqueName).IsRequired().HasMaxLength(50);

            entity.HasIndex(e => e.UniqueName).IsUnique(); // <-- добавит уникальный индекс в БД

            entity.OwnsOne(e => e.Email, email =>
            {
                email.Property(e => e.Value)
                        .HasColumnName("Email")
                        .IsRequired()
                        .HasMaxLength(255);
                email.HasIndex(e => e.Value).IsUnique();
            });

            entity.Property(e => e.HashedPassword).IsRequired();
            entity.Property(e => e.RegistrationDate).IsRequired();
        });


        // For Chat
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreateDate).IsRequired();

            // Correct many-to-many relationship configuration
            entity.HasMany(c => c.Users) // Chat has many Users (via Chat's Users collection)
                  .WithMany(u => u.Chats) // User has many Chats (via User's Chats collection)
                  .UsingEntity(j => j.ToTable("ChatUser") // Name of the join table
                                                          // Configure the relationship from the join table to Chat
                      .HasOne(typeof(Chat)) // <--- FIXED: Use non-generic HasOne(typeof(TEntity))
                          .WithMany() // Chat has many entries in the join table
                          .HasForeignKey("ChatId") // Foreign key column in ChatUser for Chat
                          .HasConstraintName("FK_ChatUser_Chat_ChatId"),
                      // Configure the relationship from the join table to User
                      j => j.HasOne(typeof(User)) // <--- FIXED: Use non-generic HasOne(typeof(TEntity))
                          .WithMany() // User has many entries in the join table
                          .HasForeignKey("UserId") // Foreign key column in ChatUser for User
                          .HasConstraintName("FK_ChatUser_User_UserId")
                  );
        });

        // ... (Message configuration remains the same) ...
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasConversion<string>();

            entity.HasOne<Chat>()
                    .WithMany()
                    .HasForeignKey(m => m.ChatId)
                    .IsRequired();

            entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(m => m.SenderId)
                    .IsRequired();
        });
    }
}