using Common;
using MassTransit;

namespace NotificationService;

public class NotificationConsumer(ITwilioService _twilioService, IInfobip _infobip): IConsumer<Notification>
{
    public async Task Consume(ConsumeContext<Notification> context)
    {
        var r =await _infobip.SendSms();
        Console.WriteLine(r.Messages.ToString());
        // _twilioService.SendSms(context.Message.Data.Recipient, context.Message.Data.Variables["otp"]);
        
        Console.WriteLine($"[User Created] {context.Message.UserId} - {context.Message.Data.Recipient}");
    }       
}