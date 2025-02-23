namespace NotificationService;

public interface ITwilioService
{
    public void SendSms(string to, string message);

}