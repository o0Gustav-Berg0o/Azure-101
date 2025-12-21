using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;

namespace ConfigurationDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ConfigController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet("settings")]
        public IActionResult GetSettings()
        {
            object settings = new
            {
                Environment = _env.EnvironmentName,

                KeyVaultStatus = new
                {
                    Status = Program.KeyVaultStatus,
                    Error = Program.KeyVaultError,
                    ConfiguredName = _configuration["KeyVaultName"]
                },

                FromKeyVault = new
                {
                    DatabasePassword = MaskSecret(_configuration["DatabasePassword"]),
                    SendGridApiKey = MaskSecret(_configuration["SendGridApiKey"]),
                    ExternalApiUrl = _configuration["ExternalApiUrl"] ?? "NOT SET"
                },

                FromAppSettings = new
                {
                    MySettingsApiKey = MaskSecret(_configuration["MySettings:ApiKey"]),
                    MySettingsMaxRetries = _configuration["MySettings:MaxRetries"] ?? "NOT SET"
                },

                AzureInfo = new
                {
                    SiteName = Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"),
                    HasManagedIdentity = !string.IsNullOrEmpty(
                        Environment.GetEnvironmentVariable("MSI_ENDPOINT"))
                }
            };

            return Ok(settings);
        }

        [HttpGet("test-keyvault")]
        public IActionResult TestKeyVault()
        {
            string dbPassword = _configuration["DatabasePassword"];
            string apiKey = _configuration["SendGridApiKey"];
            string apiUrl = _configuration["ExternalApiUrl"];

            bool keyVaultWorking = Program.KeyVaultStatus == "Connected Successfully" &&
                                  !string.IsNullOrEmpty(dbPassword) &&
                                  !string.IsNullOrEmpty(apiKey);

            object result = new
            {
                KeyVaultStatus = Program.KeyVaultStatus,
                KeyVaultError = Program.KeyVaultError,
                KeyVaultWorking = keyVaultWorking,

                Diagnostics = new
                {
                    KeyVaultConfigured = !string.IsNullOrEmpty(_configuration["KeyVaultName"]),
                    ManagedIdentityAvailable = !string.IsNullOrEmpty(
                        Environment.GetEnvironmentVariable("MSI_ENDPOINT")),
                    Environment = _env.EnvironmentName
                },

                SecretsFound = new
                {
                    DatabasePassword = !string.IsNullOrEmpty(dbPassword),
                    SendGridApiKey = !string.IsNullOrEmpty(apiKey),
                    ExternalApiUrl = !string.IsNullOrEmpty(apiUrl)
                },

                Values = keyVaultWorking ? new
                {
                    DatabasePassword = MaskSecret(dbPassword),
                    SendGridApiKey = MaskSecret(apiKey),
                    ExternalApiUrl = apiUrl
                } : null,

                Recommendation = GetRecommendation()
            };

            return Ok(result);
        }

        private string GetRecommendation()
        {
            if (Program.KeyVaultStatus == "Connected Successfully")
            {
                return "Key Vault is working correctly!";
            }

            if (Program.KeyVaultStatus.Contains("Not Configured"))
            {
                return "Add 'KeyVaultName' to Application Settings with your Key Vault name";
            }

            if (Program.KeyVaultStatus.Contains("Invalid"))
            {
                return "Fix KeyVaultName - use only the vault name, not full URL (e.g., 'my-vault')";
            }

            if (Program.KeyVaultStatus.Contains("Authentication"))
            {
                return "Enable Managed Identity: App Service -> Identity -> System assigned -> On";
            }

            if (Program.KeyVaultStatus.Contains("Access Denied"))
            {
                return "Add Access Policy: Key Vault -> Access policies -> Create -> Add your app";
            }

            return "Check logs for more details";
        }

        private string MaskSecret(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "NOT SET";
            }

            if (value.Length <= 8)
            {
                return "****";
            }

            return value.Substring(0, 4) + "..." + value.Substring(value.Length - 4);
        }
    }
}