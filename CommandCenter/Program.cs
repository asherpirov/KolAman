using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CommandCenter.Services;
using CommandCenter.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<MongoService>();
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddSingleton<ValidatorService>();

using var host = builder.Build();

var rabbitConsumer =
    host.Services.GetRequiredService<RabbitMqConsumer>();

await rabbitConsumer.StartAsync();

await host.RunAsync();