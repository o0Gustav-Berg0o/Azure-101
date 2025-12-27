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
            var functionUrl = _config["Azure:FunctionUrl"];

            // TODO 2: Skicka HTTP POST till Function
            var content = new StringContent($"\"{data}\"", System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(functionUrl, content);

            // TODO 3: Läs response
            var result = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Function response: {result}");

            return Ok(new { functionResponse = result });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling function");
            return StatusCode(500, "Error calling function");
        }
    }
}