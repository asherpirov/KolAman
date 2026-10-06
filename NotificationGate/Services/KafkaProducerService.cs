using System;
using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationGate.Settings;

namespace NotificationGate.Services;

public class KafkaProducerService
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly string _topic;

    public KafkaProducerService(ILogger<KafkaProducerService> logger, IOptions<KafkaSettings> options)
    {
        _logger = logger;
        _topic = options.Value.Topic;
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers
        };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task<DeliveryResult<Null, string>> SendAsync(string jsonMessage)
    {
        
        var message = new Message<Null, string>
        {
            Value = jsonMessage
        };

        var result = await _producer.ProduceAsync(_topic, message);
        _logger.LogInformation("Sent message to topic '{TopicName}'", _topic);
        return result;
    }
}


