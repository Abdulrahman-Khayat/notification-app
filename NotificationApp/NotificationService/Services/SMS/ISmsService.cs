namespace NotificationService.Services;

public interface ISmsService
{
    public Task<bool> SendSmsAsync(string to, string message);
}