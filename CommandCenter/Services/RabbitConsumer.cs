using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using CommandCenter.Settings;
using CommandCenter.Models;

namespace CommandCenter.Services;

public class RabbitMqConsumer
{
    private readonly RabbitMqSettings _settings;
    private readonly MongoService _mongoService;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly ValidatorService _validator;

    public RabbitMqConsumer(
        IOptions<RabbitMqSettings> options,
        MongoService mongoService,
        ILogger<RabbitMqConsumer> logger,
        ValidatorService validator)
    {
        _settings = options.Value;
        _mongoService = mongoService;
        _logger = logger;
        _validator = validator;
    }

    public async Task StartAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        foreach (var queue in _settings.Queues)
        {
            await channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, eventArgs) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(
                        eventArgs.Body.ToArray());

                    var alert = JsonSerializer.Deserialize<Alert>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (!_validator.Validate(alert))
                    {
                        await channel.BasicNackAsync(
                            eventArgs.DeliveryTag,
                            false,
                            false);

                        return;
                    }

                    alert!.Command = queue;

                    await _mongoService.InsertAlertAsync(alert);

                    await channel.BasicAckAsync(
                        eventArgs.DeliveryTag,
                        false);

                    _logger.LogInformation(
                        "Saved alert {AlertId} to {Command}",
                        alert.AlertId,
                        queue);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to process message");

                    await channel.BasicNackAsync(
                        eventArgs.DeliveryTag,
                        false,
                        true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: queue,
                autoAck: false,
                consumer: consumer);

            _logger.LogInformation(
                "Started consuming queue {Queue}",
                queue);
        }
    }
}