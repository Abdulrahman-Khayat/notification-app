using System.Reflection;
using Common;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UserService.Data;
using UserService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    

builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    var redisConnectionString = "localhost:6379"; // Replace with your Redis connection string
    return ConnectionMultiplexer.Connect(redisConnectionString);
});
builder.Services.AddSingleton<IOTPService, OTPService>();

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<PasswordHasher<User>>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwt(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.Message<Notification>(m => m.SetEntityName("user-events"));
        cfg.Publish<Notification>(p => p.ExchangeType = "topic");

        cfg.ConfigureEndpoints(context);
    }); 
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();