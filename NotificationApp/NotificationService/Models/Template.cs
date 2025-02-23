namespace NotificationService.Models;

public class Template
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}