using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Common;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using UserService.Utils;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController (IUserRepo _userRepo, PasswordHasher<User> _hasher, IConfiguration _configuration, IMapper _mapper, IBus _bus, ILogger<User> _logger): ControllerBase
{
    
    [HttpGet("{id}", Name = "[controller]/GetById")]
    public async Task<ActionResult<ReadUserDto>> GetById(Guid id)
    {
        try
        {
            var result = await _userRepo.GetByIdAsync(id);
            var otp = Otp.GenerateOtp();

            await _bus.Publish(new Notification
            {
                EventType = "user_created",
                UserId = result.Id,
                Data = new NotificationData()
                {
                    Type = "sms",
                    Recipient = result.Mobile,
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

        try
        {
            var userModel = _mapper.Map<User>(user);
            userModel.Password = _hasher.HashPassword(userModel ,userModel.Password);
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
    
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto login)
    {
        try
        {
            var user = await _userRepo.GetFirstWhereAsync(m => m.Email == login.Email);
            if (user != null)
            {
                var verified = _hasher.VerifyHashedPassword(user, user.Password, login.Password);
                if (verified == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>();
                    claims.AddRange([
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Name, user.Name),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                        new Claim("Phone", user.Mobile),
                    ]);

                    return JwtTokens.GenerateJwtToken(_configuration, claims);
                }
            }

            return Unauthorized();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Problem();   
        }
        
        
    }
}