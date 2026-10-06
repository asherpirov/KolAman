// MongoService.cs
using MongoDB.Driver;
using CommandCenter.Models;
using Microsoft.Extensions.Options;
using CommandCenter.Settings;

namespace CommandCenter.Services;

public class MongoService
{
    private readonly IMongoCollection<Alert> _collection;
    private readonly MongoSettings _settings;
    public MongoService(IOptions<MongoSettings> settings)
    {
        _settings = settings.Value;  
        var client = new MongoClient(_settings.ConnectionString);
        var db = client.GetDatabase(_settings.DatabaseName);
        _collection = db.GetCollection<Alert>(_settings.CollectionName);
    }

    public Task InsertAlertAsync(Alert alert)
    {
        return _collection.InsertOneAsync(alert);
    }
}