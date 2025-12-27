using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

namespace SlutProv.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlobController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<BlobController> _logger;

    public BlobController(IConfiguration config, ILogger<BlobController> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Ladda upp en fil till Blob Storage
    /// </summary>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        try
        {
            // TODO 1: Skapa BlobServiceClient med connection string från config
            var connectionString = _config["Azure:BlobStorage"];
            var blobServiceClient = new BlobServiceClient(connectionString);

            // TODO 2: Hämta container
            var containerClient = blobServiceClient.GetBlobContainerClient("images");

            // TODO 3: Generera unikt blob-namn
            var blobName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            // TODO 4: Ladda upp filen
            await blobClient.UploadAsync(file.OpenReadStream(), overwrite: true);

            _logger.LogInformation($"Uploaded blob: {blobName}");

            return Ok(new
            {
                blobName = blobName,
                url = blobClient.Uri.ToString(),
                message = "File uploaded successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, "Error uploading file");
        }
    }

    /// <summary>
    /// Lista alla filer i Blob Storage
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        try
        {
            // TODO 5: Skapa BlobServiceClient
            var connectionString = _config["Azure:BlobStorage"];
            var blobServiceClient = new BlobServiceClient(connectionString);

            // TODO 6: Hämta container
            var containerClient = blobServiceClient.GetBlobContainerClient("images");

            // TODO 7: Lista alla blobs
            var blobs = new List<object>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                blobs.Add(new
                {
                    name = blobItem.Name,
                    size = blobItem.Properties.ContentLength,
                    createdOn = blobItem.Properties.CreatedOn
                });
            }

            return Ok(blobs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing blobs");
            return StatusCode(500, "Error listing blobs");
        }
    }
}