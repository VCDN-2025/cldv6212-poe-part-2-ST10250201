using System.Net;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ABCRetail.Functions.Shared;

namespace ABCRetail.Functions;

public sealed class UploadImageToBlob
{
    private readonly ILogger _log;
    public UploadImageToBlob(ILogger<UploadImageToBlob> log) => _log = log;

    [Function("UploadImageToBlob")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "images/{fileName}")] HttpRequestData req,
        string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("fileName route parameter is required.");
            return bad;
        }

        var conn = Config.Get("Storage");
        var container = Config.Get("Storage:MediaContainer");

        var containerClient = new BlobContainerClient(conn, container);
        await containerClient.CreateIfNotExistsAsync();
        var blob = containerClient.GetBlobClient(fileName);

        // copy request body to blob
        await blob.UploadAsync(req.Body, overwrite: true);

        _log.LogInformation("Uploaded blob {FileName} to container {Container}", fileName, container);

        var ok = req.CreateResponse(HttpStatusCode.Created);
        await ok.WriteStringAsync($"Uploaded {fileName}");
        return ok;
    }
}
