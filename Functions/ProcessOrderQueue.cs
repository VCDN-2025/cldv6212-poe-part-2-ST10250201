using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ABCRetail.Functions.Models;

namespace ABCRetail.Functions;

public sealed class ProcessOrderQueue
{
    private readonly ILogger _log;
    public ProcessOrderQueue(ILogger<ProcessOrderQueue> log) => _log = log;

    // Note: Queue name and connection come from app settings using %...% binding
    [Function("ProcessOrderQueue")]
    public void Run(
        [QueueTrigger("%Storage:OrderQueue%", Connection = "Storage")] string message)
    {
        var evt = JsonSerializer.Deserialize<OrderEvent>(message);
        _log.LogInformation("Processed order {OrderId} for customer {CustomerId} (sku {Sku}, qty {Qty}, total {Total})",
            evt?.OrderId, evt?.CustomerId, evt?.Sku, evt?.Quantity, evt?.Total);
    }
}
