using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

public class BackgroundTaskFunction
{
    private readonly ILogger _logger;
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    private readonly IDatabase _redisCache;

    public BackgroundTaskFunction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<BackgroundTaskFunction>();

        var cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString") ?? throw new ArgumentNullException("CosmosDBConnectionString");
        _cosmosClient = new CosmosClient(cosmosConnectionString);
        _container = _cosmosClient.GetContainer("twoRegionDatabase", "twoRegionContainer");

        var redisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString") ?? throw new ArgumentNullException("RedisConnectionString");
        var redis = ConnectionMultiplexer.Connect(redisConnectionString);
        _redisCache = redis.GetDatabase();
    }

    [Function("BackgroundTaskFunction")]
    public async Task Run([QueueTrigger("inventory-queue", Connection = "AzureWebJobsStorage")] string itemJson)
    {
        try
        {
            var item = JsonConvert.DeserializeObject<InventoryItem>(itemJson);
            if (item == null)
            {
                _logger.LogWarning("Received null inventory item data for processing.");
                return;
            }

            // Update or insert item into Cosmos DB
            await _container.UpsertItemAsync(item, new PartitionKey(item.Id));
            _logger.LogInformation($"Processed item with ID {item.Id}.");

            // Update cache with latest data
            await _redisCache.StringSetAsync(item.Id, JsonConvert.SerializeObject(item));
            _logger.LogInformation($"Updated cache for item with ID {item.Id}.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing inventory item: {ex.Message}");
        }
    }
}
