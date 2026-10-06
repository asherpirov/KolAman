using MongoDB.Driver;
using CommandCenter.Models;
using Microsoft.Extensions.Options;
using CommandCenter.Settings;

namespace CommandCenter.Services;

public class MongoService
{
    private readonly IMongoCollection<Alert> _collection;

    public MongoService(IOptions<MongoSettings> settings)
    {
        var mongoSettings = settings.Value;

        var client = new MongoClient(
            mongoSettings.ConnectionString);

        var db = client.GetDatabase(
            mongoSettings.DatabaseName);

        _collection = db.GetCollection<Alert>(
            mongoSettings.CollectionName);
    }

    public Task InsertAlertAsync(Alert alert)
    {
        return _collection.InsertOneAsync(alert);
    }

    public async Task<List<Alert>> GetNewAlertsAsync()
    {
        return await _collection
            .Find(a => a.Status == "NEW")
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(string alertId, string status)
    {
        var filter = Builders<Alert>.Filter.Eq(
            a => a.AlertId,
            alertId);

        var update = Builders<Alert>.Update.Set(
            a => a.Status,
            status);

        await _collection.UpdateOneAsync(
            filter,
            update);
    }
}