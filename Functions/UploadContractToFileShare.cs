using System.Net;
using ABCRetail.Functions.Shared;
using Azure;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ABCRetail.Functions;

public sealed class UploadContractToFileShare
{
    private readonly ILogger _log;
    public UploadContractToFileShare(ILogger<UploadContractToFileShare> log) => _log = log;

    [Function("UploadContractToFileShare")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "contracts/{fileName}")] HttpRequestData req,
        string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("fileName route parameter is required.");
            return bad;
        }

        var conn = Config.Get("Storage");
        var share = Config.Get("Storage:ContractsShare");

        var shareClient = new ShareClient(conn, share);
        await shareClient.CreateIfNotExistsAsync();

        // Optional: put everything inside a folder
        var dir = shareClient.GetDirectoryClient("uploads");
        await dir.CreateIfNotExistsAsync();

        var file = dir.GetFileClient(fileName);

        // upload: we must set the length first for file shares
        using var mem = new MemoryStream();
        await req.Body.CopyToAsync(mem);
        mem.Position = 0;

        await file.CreateAsync(mem.Length);
        await file.UploadRangeAsync(new HttpRange(0, mem.Length), mem);

        _log.LogInformation("Uploaded contract {FileName} to file share {Share}", fileName, share);

        var ok = req.CreateResponse(HttpStatusCode.Created);
        await ok.WriteStringAsync($"Uploaded {fileName}");
        return ok;
    }
}
