using System.Reflection;
using Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<ConnectionMultiplexer>(sp =>
{
    var redisConnectionString = "localhost:6379"; // Replace with your Redis connection string
    return ConnectionMultiplexer.Connect(redisConnectionString);
});
builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddCustomJwt(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context,cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.Message<Notification>(m => m.SetEntityName("payment-events"));
        cfg.Publish<Notification>(p => p.ExchangeType = "topic");

        cfg.ConfigureEndpoints(context);
    }); 
});
var app = builder.Build();

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