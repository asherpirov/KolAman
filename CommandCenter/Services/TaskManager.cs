using CommandCenter.Models;
using Microsoft.Extensions.Logging;

namespace CommandCenter.Services;

public class TaskManagerService
{
    private readonly MongoService _mongo;

    public TaskManagerService(
        MongoService mongo)
    {
        _mongo = mongo;
    }

}
