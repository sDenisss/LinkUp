using LinkUp.API;
using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Infrastructure;
using LinkUp.Infrastructure.Persistence.Repositories;
using LinkUp.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LinkUp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Добавляем поддержку Razor Pages
        builder.Services.AddRazorPages();
        builder.Services.AddControllers(); // <== ОБЯЗАТЕЛЬНО
        builder.Services.AddSignalR();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddMediatR(cfg =>
             cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());


        builder.Services.AddTransient<IPasswordHasher, PasswordService>();
        // builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();

        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var config = builder.Configuration;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["Jwt:Audience"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });

        builder.Services.AddAuthorization();

       
       
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseDefaultFiles(); // Автоматически ищет index.html
        app.UseStaticFiles();  // Разрешает доступ к wwwroot

        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (AppException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(ex.Message);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync("Internal server error");
                // Логировать можно тут
            }
        });


        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();  // Маршрутизация для Razor Pages

        // API-эндпоинты (остаются рабочими!)
        app.MapControllers(); // <== ОБЯЗАТЕЛЬНО для контроллеров

        app.MapHub<ChatHub>("/chatHub");
        // app.MapFallbackToFile("index.html");

        app.Run();
    }
}


