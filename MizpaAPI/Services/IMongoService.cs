using MizpaAPI.Models;

namespace MizpaAPI.Services;

public interface IMongoService
{
    public Task<IEnumerable<AlertCommandCountDto>> GetAlertsCommandsAsync();
    
}
