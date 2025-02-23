using Common.Models;

namespace PaymentService.Models;

public class Payment: BaseModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
}