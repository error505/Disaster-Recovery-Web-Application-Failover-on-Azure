using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

public class RedisReplicationFunction
{
    private readonly ILogger _logger;
    private readonly IDatabase _primaryRedisCache;
    private readonly IDatabase _secondaryRedisCache;

    public RedisReplicationFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<RedisReplicationFunction>();

        // Connect to the primary Redis cache
        var primaryRedisConnectionString = Environment.GetEnvironmentVariable("PrimaryRedisConnectionString");
        var primaryRedis = ConnectionMultiplexer.Connect(primaryRedisConnectionString);
        _primaryRedisCache = primaryRedis.GetDatabase();

        // Connect to the secondary Redis cache
        var secondaryRedisConnectionString = Environment.GetEnvironmentVariable("SecondaryRedisConnectionString");
        var secondaryRedis = ConnectionMultiplexer.Connect(secondaryRedisConnectionString);
        _secondaryRedisCache = secondaryRedis.GetDatabase();
    }

    [Function("RedisReplicationFunction")]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo)
    {
        _logger.LogInformation("Redis replication function triggered.");

        try
        {
            // Logic to replicate data between the two Redis caches
            // This is a simplified example. You may need to implement a more complex replication logic
            var keys = _primaryRedisCache.Execute("KEYS", "*").ToStringArray();
            foreach (var key in keys)
            {
                var value = await _primaryRedisCache.StringGetAsync(key);
                await _secondaryRedisCache.StringSetAsync(key, value);
                _logger.LogInformation($"Replicated key '{key}' to secondary Redis.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error replicating data between Redis caches: {ex.Message}");
        }
    }
}
