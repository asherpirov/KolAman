using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationGate.Settings;


namespace NotificationGate.Services;

public class KafkaTopicManager
{
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaTopicManager> _logger;

    public KafkaTopicManager(ILogger<KafkaTopicManager> logger, IOptions<KafkaSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    public async Task EnsureTopicExistsAsync(int numPartitions = 1, short replicationFactor = 1)
    {
        var topicName = _settings.Topic;
        var config = new AdminClientConfig
        {
            BootstrapServers = _settings.BootstrapServers
        };

        using var adminClient = new AdminClientBuilder(config).Build();
        try
        {
            await adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = topicName,
                    NumPartitions = numPartitions,
                    ReplicationFactor = replicationFactor
                }
            });
            _logger.LogInformation("Topic '{TopicName}' created successfully.", topicName);
        }
        catch (CreateTopicsException e)
        {
            if (e.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
            {
                _logger.LogInformation("Topic '{TopicName}' already exists.", topicName);
            }
            else
            {
                throw new Exception($"Failed to create topic: {e.Results[0].Error.Reason}");
            }
        }
    }
}