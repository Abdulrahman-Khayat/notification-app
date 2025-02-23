using System.Text.RegularExpressions;
using Common;
using MassTransit;
using NotificationService.Data;
using NotificationService.Services;

namespace NotificationService;

public class UserConsumer(ISmsService _smsService, IEmailService _emailService, ITemplateRepo _templateRepo): IConsumer<Notification>
{
    public async Task Consume(ConsumeContext<Notification> context)
    {
        var template = await _templateRepo.GetFirstWhereAsync(t => t.Name == context.Message.EventType);
        if (template != null)
        {
            var body = Regex.Replace(template.Content, @"<(\w+)>", match =>
            {
                string key = match.Groups[1].Value; // Extract var name without <>
                return context.Message.Data.Variables.TryGetValue(key, out string value) ? value : match.Value;
            });
            
            await _emailService.SendEmailAsync(context.Message.Data.Recipient, template.Subject, body);
        }
        
        Console.WriteLine($"[User Created] {context.Message.UserId} - {context.Message.Data.Recipient}");
    }       
}