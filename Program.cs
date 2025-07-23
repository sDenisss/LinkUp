using LinkUp.API;
using LinkUp.Application;
using LinkUp.Domain;
using LinkUp.Infrastructure;
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
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();

        builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Your existing TokenValidationParameters go here (Issuer, Audience, SymmetricSecurityKey, etc.)
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };

                // THIS IS CRUCIAL FOR SIGNALR: Extract token from query string
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"]; // Attempt to get token from query string
                        var path = context.HttpContext.Request.Path;

                        // Check if the request is for the SignalR hub path
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chatHub"))) // Adjust "/chatHub" to your actual hub path
                        {
                            context.Token = accessToken; // Assign the token for validation
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        // Add Authorization policies if you have any, otherwise just AddAuthorization()
        builder.Services.AddAuthorization(); // Required for [Authorize] attribute

        // Add SignalR
        builder.Services.AddSignalR();

        // ... other services ...

        // In app.Use... section (or Configure in Startup.cs)
        // app.UseAuthorization();  // Must be *before* MapHub() for authorization to work

        // // Map your SignalR hub
        // app.MapHub<ChatHub>("/chatHub"); // Adjust "/chatHub" to your actual hub path

       
       
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
        app.UseAuthentication(); // Must be *before* UseAuthorization()
        app.UseAuthorization();
        app.MapRazorPages();  // Маршрутизация для Razor Pages

        // API-эндпоинты (остаются рабочими!)
        app.MapControllers(); // <== ОБЯЗАТЕЛЬНО для контроллеров

        app.MapHub<ChatHub>("/chatHub");
        // app.MapFallbackToFile("index.html");

        app.Run();
    }
}




// public class Program
// {
//     public static void Main(string[] args)
//     {
//         var builder = WebApplication.CreateBuilder(args);

//         builder.Services.AddDbContext<ApplicationDbContext>(options =>
//             options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//         // Добавляем поддержку Razor Pages
//         // builder.Services.AddRazorPages();
//         builder.Services.AddCors(options =>
//         {
//             options.AddPolicy("AllowSpecificOrigin",
//                 builder => builder.WithOrigins("http://localhost:3000", "http://localhost:5037") // Replace 3000 with your React app's port
//                                 .AllowAnyHeader()
//                                 .AllowAnyMethod()
//                                 .AllowCredentials());
//         });
//         builder.Services.AddControllers(); // <== ОБЯЗАТЕЛЬНО
//         builder.Services.AddSignalR();

//         builder.Services.AddEndpointsApiExplorer();
//         builder.Services.AddSwaggerGen();

//         // builder.Services.AddMediatR(cfg =>
//         //      cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());
//         // builder.Services.AddMediatR(cfg =>
//         //         cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
//         // builder.Services.AddMediatR(cfg =>
//         // {
//         //     cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg0NDE5MjAwIiwiaWF0IjoiMTc1MjkyNjIxNyIsImFjY291bnRfaWQiOiIwMTk4MjI3MTMyN2Q3NTkxODZkMzI5YWJmNzdmMDAwYyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazBoOHBzZzM5emJhODE5YnJ6d2FiNWE0Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.CKZt6kngXXyGip2N-rYb2k9_TDEUtKIJvG3Bq9FJALOqeiJ09gehz5ncWga2W3X3W-AvyE_7VsJxn6iMKv_W1L34yq38srswH6C7BGsi9FnXbZ0aftbawtvnghJ9WvkxDau1jV5-Y-oGjcAKfUWg7l1z60FjJj_blo9qRyg004_HuVNesbPT-nYEWmrMT1FvNPwhGLfQ8z4rQNwaTJkEGaHditFReHt05VupS5VZpn3eXwtkPxRnp5JlZuChBOGBI3H0mY-mkMLyOiubQrQIaT-jO_ULQiXVWQ4QwIgTib_FbKbclr1V3R4uSdLeox4Ya631bcLbX4hFqQ9frT86Tw";
//         // });

//         builder.Services.AddTransient<IPasswordHasher, PasswordService>();
//         // builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
//         builder.Services.AddScoped<IChatRepository, ChatRepository>();
//         builder.Services.AddScoped<IMessageRepository, MessageRepository>();

//         builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

//         builder.Services
//             .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//             .AddJwtBearer(options =>
//             {
//                 var config = builder.Configuration;
//                 options.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidateIssuer = true,
//                     ValidIssuer = config["Jwt:Issuer"],
//                     ValidateAudience = true,
//                     ValidAudience = config["Jwt:Audience"],
//                     ValidateLifetime = true,
//                     IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"])),
//                     ValidateIssuerSigningKey = true
//                 };
//             });

//         builder.Services.AddAuthorization();

       
       
//         var app = builder.Build();

//         if (app.Environment.IsDevelopment())
//         {
//             app.UseSwagger();
//             app.UseSwaggerUI();
//         }

//         app.UseHttpsRedirection();
//         app.UseStaticFiles();

//         app.UseDefaultFiles(); // Автоматически ищет index.html
//         app.UseStaticFiles();  // Разрешает доступ к wwwroot

//         app.UseCors("AllowSpecificOrigin");

//         app.Use(async (context, next) =>
//         {
//             try
//             {
//                 await next();
//             }
//             catch (AppException ex)
//             {
//                 context.Response.StatusCode = ex.StatusCode;
//                 context.Response.ContentType = "text/plain";
//                 await context.Response.WriteAsync(ex.Message);
//             }
//             catch (Exception ex)
//             {
//                 context.Response.StatusCode = 500;
//                 context.Response.ContentType = "text/plain";
//                 await context.Response.WriteAsync("Internal server error");
//                 // Логировать можно тут
//             }
//         });


//         app.UseRouting();
//         app.UseAuthorization();
//         // app.MapRazorPages();  // Маршрутизация для Razor Pages

//         // API-эндпоинты (остаются рабочими!)
//         app.MapControllers(); // <== ОБЯЗАТЕЛЬНО для контроллеров

//         app.MapHub<ChatHub>("/chatHub");
//         // app.MapFallbackToFile("index.html");

//         app.Run();
//     }
// }


