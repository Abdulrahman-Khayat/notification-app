using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Common;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Dtos;
using UserService.Models;
using UserService.Utils;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController (IUserRepo _userRepo, PasswordHasher<User> _hasher, IConfiguration _configuration, IMapper _mapper, IBus _bus, ILogger<User> _logger, IOTPService _otpService): ControllerBase
{
    
    [HttpGet("{id}", Name = "[controller]/GetById")]
    public async Task<ActionResult<ReadUserDto>> GetById(Guid id)
    {
        try
        {
            var result = await _userRepo.GetByIdAsync(id);
            
            // this only for making testing easier
            var otp = _otpService.GenerateOTP(result.Id, "user");
            if (otp != null)
            {
                await _bus.Publish(new Notification
                {
                    EventType = "welcome_template",
                    UserId = result.Id,
                    Data = new NotificationData()
                    {
                        Type = "email",
                        Recipient = result.Email,
                        Variables = new Dictionary<string, string>
                        {
                            { "Name", result.Name },
                            { "otp", otp }
                        }
                    }
                }, context =>
                {
                    context.SetRoutingKey("user.events.created");
                });
            }
            return Ok(_mapper.Map<ReadUserDto>(result));
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
            
            var otp = _otpService.GenerateOTP(result.Id, "user");
            await _bus.Publish(new Notification
            {
                EventType = "welcome_template",
                UserId = result.Id,
                Data = new NotificationData()
                {
                    Type = "email",
                    Recipient = user.Email,
                    Variables = new Dictionary<string, string>
                    {
                        { "Name", user.Name },
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
                    ]);
                    var otp = _otpService.GenerateOTP(user.Id, "user");
                    
                    if (otp != null)
                    {
                        await _bus.Publish(new Notification
                        {
                            EventType = "welcome_template",
                            UserId = user.Id,
                            Data = new NotificationData()
                            {
                                Type = "email",
                                Recipient = user.Email,
                                Variables = new Dictionary<string, string>
                                {
                                    { "Name", user.Name},
                                    { "otp", otp }
                                }
                            }
                        }, context =>
                        {
                            context.SetRoutingKey("user.events.login");
                        });
                    }
                    // todo: fix this
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

    [Authorize]
    [HttpPost("verify")]
    public async Task<ActionResult<string>> Verify(string otp)
    {
        var userId = HttpContext.User.FindFirst("sub")?.Value;

        var result = _otpService.ValidateOTP(Guid.Parse(userId), otp, "user");

        if (result)
        {
            var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));
            var claims = new List<Claim>();
            claims.AddRange([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Phone", user.Mobile),
            ]);
            
            _otpService.DeleteOTP(user.Id, "user");
            return JwtTokens.GenerateJwtToken(_configuration, claims);
        }

        return Unauthorized();
    }

    [Authorize]
    [HttpPost("regenerate-otp")]
    public async Task<ActionResult<string>> Regenerate()
    {
        var userId = HttpContext.User.FindFirst("sub")?.Value;

        var otp = _otpService.GenerateOTP(Guid.Parse(userId), "user");

        if (otp != null)
        {
            var user = await _userRepo.GetByIdAsync(Guid.Parse(userId));

            await _bus.Publish(new Notification
            {
                EventType = "regenerate_template",
                UserId = user.Id,
                Data = new NotificationData()
                {
                    Type = "email",
                    Recipient = user.Email,
                    Variables = new Dictionary<string, string>
                    {
                        { "Name", user.Name },
                        { "otp", otp }
                    }
                }
            }, context =>
            {
                context.SetRoutingKey("user.events.regenerate");
            });
            return Accepted();
        }

        return BadRequest("Cannot generate OTP");
    }

}