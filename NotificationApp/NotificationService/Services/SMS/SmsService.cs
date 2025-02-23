namespace NotificationService.Services;

public class SmsService(TwilioService _twilioService, InfobibService _infobibService): ISmsService
{
    public async Task<bool> SendSmsAsync(string to, string message)
    {
        if (await _twilioService.SendSmsAsync(to, message))
        {
            return true;
        }

        Console.WriteLine("Primary SMS service failed, using secondary service...");
        return await _infobibService.SendSmsAsync(to, message);
    }
}