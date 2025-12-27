using Microsoft.AspNetCore.Mvc;

namespace SlutProv.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FunctionController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<FunctionController> _logger;
    private readonly HttpClient _httpClient;

    public FunctionController(IConfiguration config, ILogger<FunctionController> logger, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Anropa Azure Function
    /// </summary>
    [HttpPost("trigger")]
    public async Task<IActionResult> Trigger([FromBody] string data)
    {
        try
        {
            // TODO 1: Hämta Function URL från config

            // TODO 2: Skicka HTTP POST till Function
          

            // TODO 3: Läs response

            _logger.LogInformation($"Function response: result");

            return Ok(new { functionResponse = "result" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling function");
            return StatusCode(500, "Error calling function");
        }
    }
}