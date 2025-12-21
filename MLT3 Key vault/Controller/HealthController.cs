using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;

namespace ConfigurationDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HealthController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            object health = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = _env.EnvironmentName,
                ApplicationName = _env.ApplicationName,
                MachineName = Environment.MachineName,

                Checks = new
                {
                    ConfigurationLoaded = _configuration != null,
                    KeyVaultConfigured = !string.IsNullOrEmpty(_configuration["KeyVaultName"]),
                    ManagedIdentityAvailable = !string.IsNullOrEmpty(
                        Environment.GetEnvironmentVariable("MSI_ENDPOINT"))
                }
            };

            return Ok(health);
        }

        [HttpGet("ready")]
        public IActionResult Ready()
        {
            bool isReady = true;

            // Kolla om Key Vault är tillgänglig
            string keyVaultName = _configuration["KeyVaultName"];
            if (!string.IsNullOrEmpty(keyVaultName))
            {
                string dbPassword = _configuration["DatabasePassword"];
                isReady = !string.IsNullOrEmpty(dbPassword);
            }

            if (isReady)
            {
                return Ok(new { status = "ready" });
            }
            else
            {
                return StatusCode(503, new { status = "not ready" });
            }
        }
    }
}