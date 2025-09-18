using LinkUp.API;
using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Infrastructure;
using LinkUp.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LinkUp;

public class Program
{
    public static void Main(string[] args)
    {
        // Загружаем .env файл
        DotNetEnv.Env.Load();
        
        var builder = WebApplication.CreateBuilder(args);

        // Получаем значения из переменных окружения
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var dbUser = Environment.GetEnvironmentVariable("DB_USER");
        var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        var dbName = Environment.GetEnvironmentVariable("DB_DB");


        // var machineName = Environment.MachineName;
        // var processId = Environment.ProcessId;
        // Response.Headers.Add("X-Server-Id", $"{machineName}-{processId}");
        
        // Формируем connection string из переменных окружения
        var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

        // Логируем для проверки (без пароля)
        // Console.WriteLine($"DB Connection: Host={dbHost}, Port={dbPort}, Database={dbName}, User={dbUser}");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Получаем JWT настройки из переменных окружения
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");

        // Проверяем что все переменные есть
        if (string.IsNullOrEmpty(jwtKey))
            throw new Exception("JWT_KEY is not set in environment variables");
        if (string.IsNullOrEmpty(jwtIssuer))
            throw new Exception("JWT_ISSUER is not set in environment variables");
        if (string.IsNullOrEmpty(jwtAudience))
            throw new Exception("JWT_AUDIENCE is not set in environment variables");

        // Добавляем поддержку Razor Pages
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        builder.Services.AddSignalR();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddMediatR(cfg =>
             cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

        builder.Services.AddTransient<IPasswordHasher, PasswordService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddSignalR();
       
        var app = builder.Build();



        // // Автоматическое применение миграций
        // using (var scope = app.Services.CreateScope())
        // {
        //     var services = scope.ServiceProvider;
        //     try
        //     {
        //         var context = services.GetRequiredService<ApplicationDbContext>();
        //         context.Database.Migrate(); // Применяет все pending миграции
        //         Console.WriteLine("Migrations applied successfully");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error applying migrations: {ex.Message}");
        //         // Не бросаем исключение, чтобы приложение могло запуститься
        //     }
        // }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();
            // ! this block code only for docker-compose build of responces out of server / with this block doesnt work dotnet run
            // app.Use(async (context, next) =>
            // {
            //     // Вызываем следующий middleware в цепочке (это важно!)
            //     await next();

            //     // После того, как ответ сгенерирован, добавляем свой заголовок
            //     var serverId = Environment.GetEnvironmentVariable("SERVER_ID") ?? "unknown";
            //     context.Response.Headers.Append("X-Server-ID", serverId);
            // });
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseDefaultFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapRazorPages();
        app.MapControllers();
        app.MapHub<ChatHub>("/chatHub");

        app.Run();
    }
}