
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SlutProv.Api.Data;
using SlutProv.Api.Models;

namespace SlutProv.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly ImageDbContext _context;
    private readonly ILogger<DatabaseController> _logger;

    public DatabaseController(ImageDbContext context, ILogger<DatabaseController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Spara metadata i databasen
    /// </summary>
    /// <param name="request">Bildmetadata att spara (endast fileName och blobUrl)</param>
    /// <returns>Den sparade bildmetadatan med genererat ID och uploadedAt</returns>
    /// <remarks>
    /// Exempel:
    /// 
    ///     POST /api/database/create
    ///     {
    ///         "fileName": "sunset.jpg",
    ///         "blobUrl": "https://stimageservicetest.blob.core.windows.net/images/sunset.jpg"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Metadata sparades framgångsrikt</response>
    /// <response code="400">Ogiltig input</response>
    /// <response code="500">Serverfel</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ImageMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] ImageMetadata metadata)
    {
        try
        {
            // TODO 1: Sätt UploadedAt till nu

            // TODO 2: Lägg till i context

            // TODO 3: Spara ändringar

            _logger.LogInformation($"Created image metadata with ID: {metadata.Id}");

            return Ok(metadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating metadata");
            return StatusCode(500, "Error creating metadata");
        }
    }

    /// <summary>
    /// Hämta alla metadata från databasen
    /// </summary>
    /// <returns>Lista med bildmetadata, sorterade efter uppladdningsdatum (nyast först)</returns>
    /// <response code="200">Lista returnerades framgångsrikt</response>
    /// <response code="500">Serverfel</response>
    [HttpGet("list")]
    [ProducesResponseType(typeof(List<ImageMetadata>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> List()
    {
        try
        {
            // TODO 4: Hämta alla images, sortera på UploadedAt descending
           

            return Ok("images");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing metadata");
            return StatusCode(500, "Error listing metadata");
        }
    }
}