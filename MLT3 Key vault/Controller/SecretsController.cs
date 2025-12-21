using ConfigurationDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConfigurationDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecretsController : ControllerBase
    {
        private readonly IKeyVaultService _keyVaultService;

        public SecretsController(IKeyVaultService keyVaultService)
        {
            _keyVaultService = keyVaultService;
        }

        [HttpGet("{secretName}")]
        public async Task<IActionResult> GetSecret(string secretName)
        {
            try
            {
                string secretValue = await _keyVaultService.GetSecretAsync(secretName);

                return Ok(new
                {
                    SecretName = secretName,
                    Value = MaskSecret(secretValue),
                    Retrieved = true
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = ex.Message,
                    SecretName = secretName
                });
            }
        }

        [HttpGet("{secretName}/exists")]
        public async Task<IActionResult> CheckSecretExists(string secretName)
        {
            bool exists = await _keyVaultService.SecretExistsAsync(secretName);

            return Ok(new
            {
                SecretName = secretName,
                Exists = exists
            });
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