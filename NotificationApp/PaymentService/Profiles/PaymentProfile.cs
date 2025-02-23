using AutoMapper;
using PaymentService.Data;
using PaymentService.Dtos;
using PaymentService.Models;

namespace PaymentService.Profiles;

public class PaymentProfile: Profile
{
    public PaymentProfile()
    {
        CreateMap<Payment, ReadPaymentDto>();
        CreateMap<CreatePaymentDto, Payment>();
    }
}