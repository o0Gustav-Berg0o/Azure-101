
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
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ImageMetadata metadata)
    {
        try
        {
            // TODO 1: Sätt UploadedAt till nu
            metadata.UploadedAt = DateTime.UtcNow;

            // TODO 2: Lägg till i context
            _context.Images.Add(metadata);

            // TODO 3: Spara ändringar
            await _context.SaveChangesAsync();

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
    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        try
        {
            // TODO 4: Hämta alla images, sortera på UploadedAt descending
            var images = await _context.Images
                .OrderByDescending(i => i.UploadedAt)
                .ToListAsync();

            return Ok(images);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing metadata");
            return StatusCode(500, "Error listing metadata");
        }
    }
}