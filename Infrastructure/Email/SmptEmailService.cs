// MessengerApp.Infrastructure/Email/SmtpEmailService.cs
using System.Net.Mail;
using System.Net;
using LinkUp.Application;

namespace LinkUp.Infrastructure;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;
    private readonly string _fromEmail;

    public SmtpEmailService(string smtpHost, int smtpPort, string username, string password, string fromEmail)
    {
        _smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true // Обычно true для продакшн
        };
        _fromEmail = fromEmail;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailMessage = new MailMessage(_fromEmail, toEmail, subject, body);
        await _smtpClient.SendMailAsync(mailMessage);
        // В реальном приложении здесь можно добавить логирование
        // или обработку ошибок отправки (например, через Polly для ретраев)
    }
}
