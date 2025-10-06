using System.Net;
using Azure;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ABCRetail.Functions.Models;
using ABCRetail.Functions.Shared;

namespace ABCRetail.Functions;

public sealed class StoreCustomerToTable
{
    private readonly ILogger _log;
    public StoreCustomerToTable(ILogger<StoreCustomerToTable> log) => _log = log;

    [Function("StoreCustomerToTable")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "customers")] HttpRequestData req)
    {
        var customer = await req.ReadFromJsonAsync<Customer>();
        if (customer is null || string.IsNullOrWhiteSpace(customer.CustomerId))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid customer payload.");
            return bad;
        }

        var conn = Config.Get("Storage");
        var tableName = Config.Get("Storage:CustomersTable");

        var table = new TableClient(conn, tableName);
        await table.CreateIfNotExistsAsync();

        var entity = new TableEntity(partitionKey: "CUSTOMER", rowKey: customer.CustomerId)
        {
            { "FirstName", customer.FirstName ?? "" },
            { "LastName",  customer.LastName  ?? "" },
            { "Email",     customer.Email     ?? "" }
        };

        await table.UpsertEntityAsync(entity, TableUpdateMode.Replace);

        _log.LogInformation("Stored customer {CustomerId} ({Email})", customer.CustomerId, customer.Email);

        var ok = req.CreateResponse(HttpStatusCode.OK);
        await ok.WriteStringAsync("Customer stored.");
        return ok;
    }
}
