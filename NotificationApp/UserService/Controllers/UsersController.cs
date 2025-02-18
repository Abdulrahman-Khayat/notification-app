using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController (IUserRepo _userRepo, IMapper _mapper, IBus _bus, ILogger<User> _logger): ControllerBase
{
    
    [HttpGet("{id}", Name = "[controller]/GetById")]
    public async Task<ActionResult<ReadUserDto>> GetById(Guid id)
    {
        try
        {
            var result = await _userRepo.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Problem();
        }
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<ReadUserDto>> Create(CreateUserDto user)
    {

        var x = Otp.GenerateOtp();
        try
        {
            var userModel = _mapper.Map<User>(user);
            var result = await _userRepo.AddAsync(userModel);
            
            await _userRepo.SaveChangesAsync();
            
            var otp = Otp.GenerateOtp();
            await _bus.Publish(new Notification
            {
                EventType = "user_created",
                UserId = result.Id,
                Data = new NotificationData()
                {
                    Type = "sms",
                    Recipient = user.Mobile,
                    Variables = new Dictionary<string, string>
                    {
                        { "Name", "John" },
                        { "otp", otp }
                    }
                }
            }, context =>
            {
                context.SetRoutingKey("user.events.created");
            });
            
            var route = this.ControllerContext.ActionDescriptor?.ControllerName + "/GetById";
            return CreatedAtRoute(route, new { result.Id }, _mapper.Map<ReadUserDto>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Problem();
        }

    }
}