using CommerceHub.API.Settings;
using CommerceHub.Application.Interfaces;
using CommerceHub.Application.Services;
using CommerceHub.Infrastructure.Data;
using CommerceHub.Infrastructure.Repositories;
using CommerceHub.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// --------------------
// MongoDB Registration
// --------------------
builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var mongoSettings = configuration
        .GetSection("MongoSettings")
        .Get<MongoSettings>();

    if (mongoSettings == null)
        throw new Exception("MongoSettings not found in appsettings.json");

    return new MongoDbContext(
        mongoSettings.ConnectionString,
        mongoSettings.DatabaseName
    );
});

// --------------------
// RabbitMQ Registration
// --------------------
builder.Services.Configure<RabbitMqSettings>(
    configuration.GetSection("RabbitMqSettings")
);

builder.Services.AddSingleton<IMessagePublisher>(sp =>
{
    var settings = configuration
        .GetSection("RabbitMqSettings")
        .Get<RabbitMqSettings>();

    if (settings == null)
        throw new Exception("RabbitMqSettings not found in appsettings.json");

    return new RabbitMqPublisher(settings);
});

// --------------------
// Repositories
// --------------------
builder.Services.AddScoped<IOrderRepository, MongoOrderRepository>();
builder.Services.AddScoped<IProductRepository, MongoProductRepository>();

// --------------------
// Application Services
// --------------------
builder.Services.AddScoped<OrderService>();

// --------------------
// Controllers + Swagger
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();