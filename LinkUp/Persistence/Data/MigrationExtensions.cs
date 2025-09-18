using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

namespace LinkUp.Persistence;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder applicationBuilder)
    {
        try
        {
            using IServiceScope scope = applicationBuilder.ApplicationServices.CreateScope();
            using ApplicationDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Database.Migrate();
            System.Console.WriteLine("Migrate successefully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations: {ex.Message}");
            // Не прерываем выполнение, приложение может работать в режиме без БД
        }
    }
}