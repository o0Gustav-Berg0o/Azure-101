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
          

            // TODO 2: Hämta container

            // TODO 3: Generera unikt blob-namn
       
            // TODO 4: Ladda upp filen

            _logger.LogInformation($"Uploaded blob: blobName");

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
            

            // TODO 6: Hämta container

            // TODO 7: Lista alla blobs
            

            return Ok("blobs");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing blobs");
            return StatusCode(500, "Error listing blobs");
        }
    }
}