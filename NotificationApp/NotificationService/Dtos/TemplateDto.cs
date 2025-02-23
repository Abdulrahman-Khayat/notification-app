namespace TemplateService.Dtos;

public class ReadTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}
public class CreateTemplateDto
{
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}
