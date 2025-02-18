namespace Common;

public class Notification
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; }
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public NotificationData Data { get; set; }
}
public class NotificationData
{
    public string Type { get; set; }  // "email", "sms", "push"
    public string Recipient { get; set; }
    public Dictionary<string, string> Variables { get; set; }
}
