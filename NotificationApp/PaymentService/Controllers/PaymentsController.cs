using System.Security.Claims;
using AutoMapper;
using Common;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using PaymentService.Data;
using PaymentService.Dtos;
using PaymentService.Models;

namespace PaymentService.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController (IPaymentRepo _paymentRepo, IConfiguration _configuration, IMapper _mapper, IBus _bus, ILogger<Payment> _logger, IOTPService _otpService): ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create(CreatePaymentDto payment)
    {
        try
        {
            var userId = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var userName = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
            var userEmail = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            var userPhone = HttpContext.User.FindFirst("Phone")?.Value;

            var paymentModel = _mapper.Map<Payment>(payment);
            
            var result = await _paymentRepo.AddAsync(paymentModel);
            await _paymentRepo.SaveChangesAsync();
 
            var otp = _otpService.GenerateOTP(Guid.Parse(userId), "payment");
            
            await _bus.Publish(new Notification
            {
                EventType = "payment_template",
                UserId = result.Id,
                Data = new NotificationData()
                {
                    Type = "email",
                    Recipient = userEmail,
                    Variables = new Dictionary<string, string>
                    {
                        { "Name", userName },
                        { "otp", otp }
                    }
                }
            }, context =>
            {
                context.SetRoutingKey("user.events.created");
            });

            return Accepted();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Problem();
        }
    }
    
    [Authorize]
    [HttpPost("{id}/Verify")]
    public async Task<ActionResult> Verify(Guid Id, string Otp)
    {
        try
        {
            var userId = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var userName = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
            var userEmail = HttpContext.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            var userPhone = HttpContext.User.FindFirst("Phone")?.Value;

            
            
            var payment = await _paymentRepo.GetByIdAsync(Id);
            
            var result = _otpService.ValidateOTP(Guid.Parse(userId), Otp, "payment");
            if (result)
            {
                payment.Status = "Accepted";
                await _paymentRepo.SaveChangesAsync();

                await _bus.Publish(new Notification
                {
                    EventType = "payment_success_template",
                    UserId = Guid.Parse(userId),
                    Data = new NotificationData()
                    {
                        Type = "email",
                        Recipient = userEmail,
                        Variables = new Dictionary<string, string>
                        {
                            { "Name", userName },
                            { "Amount", payment.Amount.ToString() }
                        }
                    }
                }, context =>
                {
                    context.SetRoutingKey("payment.events.success");
                });
                
                
            }
            
 
            
            

            return Accepted();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Problem();
        }
    }
   

}