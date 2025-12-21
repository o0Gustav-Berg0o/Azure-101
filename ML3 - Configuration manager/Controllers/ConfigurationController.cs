using Microsoft.AspNetCore.Mvc;

namespace ConfigurationDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ConfigController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            message = "Config controller works!",
            timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        var settings = new
        {
            ApiKey = _configuration["MySettings:ApiKey"] ?? "NOT SET",
            DatabaseConnection = _configuration["MySettings:DatabaseConnection"] ?? "NOT SET",
            FeatureEnabled = _configuration["MySettings:FeatureEnabled"] ?? "NOT SET",
            MaxRetries = _configuration["MySettings:MaxRetries"] ?? "NOT SET",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        };

        return Ok(settings);
    }
}