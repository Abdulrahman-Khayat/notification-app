using Common.Models;

namespace UserService.Models;

public class User: BaseModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string Mobile { get; set; }
}