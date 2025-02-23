using NetTopologySuite.Geometries.Prepared;

namespace PaymentService.Dtos;

public class ReadPaymentDto
{
    public Decimal Amount { get; set; }
    
}
public class CreatePaymentDto
{
    public Decimal Amount { get; set; }
}

public class VerifyPaymentDto
{
    public string Otp { get; set; }
}