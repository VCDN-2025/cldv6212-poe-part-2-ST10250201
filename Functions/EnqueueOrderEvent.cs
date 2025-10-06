using System.Net;
using System.Text.Json;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ABCRetail.Functions.Models;
using ABCRetail.Functions.Shared;

namespace ABCRetail.Functions;

public sealed class EnqueueOrderEvent
{
    private readonly ILogger _log;
    public EnqueueOrderEvent(ILogger<EnqueueOrderEvent> log) => _log = log;

    [Function("EnqueueOrderEvent")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequestData req)
    {
        var order = await req.ReadFromJsonAsync<OrderEvent>();
        if (order is null || string.IsNullOrWhiteSpace(order.OrderId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid order payload.");
            return bad;
        }

        var conn = Config.Get("Storage");
        var queueName = Config.Get("Storage:OrderQueue");

        var queue = new QueueClient(conn, queueName);
        await queue.CreateIfNotExistsAsync();

        var json = JsonSerializer.Serialize(order);
        await queue.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json)));

        _log.LogInformation("Enqueued order {OrderId} for customer {CustomerId}", order.OrderId, order.CustomerId);

        var res = req.CreateResponse(HttpStatusCode.Accepted);
        await res.WriteStringAsync("Order enqueued.");
        return res;
    }
}
