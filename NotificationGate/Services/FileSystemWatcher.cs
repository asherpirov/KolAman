using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationGate.Services;
using NotificationGate.Settings;

public class FileWatcherService
{
    private readonly KafkaProducerService _producer;
    private readonly ILogger<FileWatcherService> _logger;
    private readonly FileWatcherSettings _settings;
    private FileSystemWatcher? _watcher;

    public FileWatcherService(
    KafkaProducerService producer,
    ILogger<FileWatcherService> logger,
    IOptions<FileWatcherSettings> options)
    {
        _producer = producer;
        _logger = logger;
        _settings = options.Value;
    }

    public void Start()
    {
        _watcher = new FileSystemWatcher(_settings.AlertsPath);

        _watcher.Filter = "*.ready";
        _watcher.IncludeSubdirectories = true;
        _watcher.InternalBufferSize = 65536;

        _watcher.Created += OnCreated;
        _watcher.EnableRaisingEvents = true;
    }

    private async void OnCreated(object sender, FileSystemEventArgs e)
    {
        try
        {
            var folderPath = Path.GetDirectoryName(e.FullPath);

            if (folderPath == null)
                return;

            var alertPath = Path.Combine(folderPath, "alert.json");

            if (!File.Exists(alertPath))
                return;

            var jsonMessage = await File.ReadAllTextAsync(alertPath);

            await _producer.SendAsync(jsonMessage);

            _logger.LogInformation("Alert sent to Kafka: {alertPath}", alertPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed processing alert");
        }
    }
}
