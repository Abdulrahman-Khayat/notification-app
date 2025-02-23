namespace NotificationService.Services;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(string to, string subject, string body);
}