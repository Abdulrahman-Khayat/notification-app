using Common.Models;

namespace NotificationService.Models;

public class Template: BaseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}