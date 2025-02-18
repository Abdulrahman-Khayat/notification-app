using Common;
using MassTransit;

namespace NotificationService;

public class NotificationConsumer: IConsumer<Notification>
{
    public Task Consume(ConsumeContext<Notification> context)
    {
        Console.WriteLine($"[User Created] {context.Message.UserId} - {context.Message.Data.Recipient}");
        return Task.CompletedTask;
    }       
}