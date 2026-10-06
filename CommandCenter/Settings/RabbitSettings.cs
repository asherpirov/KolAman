using System;

namespace CommandCenter.Settings;

public class RabbitMqSettings
{
    public string Host { get; set; } = "localhost";
    public List<string> Queues { get; set; } = new();
}

