using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryController> _logger;
    private readonly CosmosClient _cosmosClient;
    private readonly Container _container;
    private readonly QueueClient _queueClient;

    public InventoryController(IHttpClientFactory httpClientFactory, ILogger<InventoryController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("FunctionClient");
        _logger = logger;

        var cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString") ?? throw new ArgumentNullException("CosmosDBConnectionString", "Cosmos DB connection string is not set.");
        _cosmosClient = new CosmosClient(cosmosConnectionString);
        _container = _cosmosClient.GetContainer("twoRegionDatabase", "twoRegionContainer");

        var queueConnectionString = Environment.GetEnvironmentVariable("AzureQueueStorageConnectionString") ?? throw new ArgumentNullException("AzureQueueStorageConnectionString", "Azure Queue Storage connection string is not set.");
        _queueClient = new QueueClient(queueConnectionString, "inventory-queue");
    }

    [HttpPost("add-item")]
    public async Task<IActionResult> AddInventoryItem([FromBody] InventoryItem item)
    {
        try
        {
            // Validate the request
            if (item == null || string.IsNullOrEmpty(item.Id) || item.Quantity < 0)
            {
                _logger.LogWarning("Invalid inventory item data.");
                return BadRequest("Invalid inventory item data.");
            }

            // Add item to Cosmos DB
            await _container.CreateItemAsync(item, new PartitionKey(item.Id));
            _logger.LogInformation($"Added item with ID {item.Id} to inventory.");

            // Queue the task for background processing
            string message = JsonConvert.SerializeObject(item);
            await _queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(message)));
            _logger.LogInformation($"Queued message for item with ID {item.Id}.");

            return Ok("Item added and queued successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding item to inventory: {ex.Message}");
            return StatusCode(500, "An error occurred while adding the item.");
        }
    }

    [HttpGet("get-item/{id}")]
    public async Task<IActionResult> GetInventoryItem(string id)
    {
        try
        {
            // Validate input
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("Item ID is missing.");
                return BadRequest("Item ID is required.");
            }

            // Retrieve item from Cosmos DB
            var response = await _container.ReadItemAsync<InventoryItem>(id, new PartitionKey(id));
            return Ok(response.Resource);
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogWarning($"Item with ID {id} not found.");
            return NotFound("Item not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving item from inventory: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the item.");
        }
    }
}
