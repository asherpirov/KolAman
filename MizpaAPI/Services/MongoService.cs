using MongoDB.Driver;
using MizpaAPI.Models;
using Microsoft.Extensions.Options;
using MizpaAPI.Settings;

namespace MizpaAPI.Services;

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
}