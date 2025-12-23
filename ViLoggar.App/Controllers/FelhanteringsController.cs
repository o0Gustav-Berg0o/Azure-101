using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ViLoggar.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FelhanteringsController : ControllerBase
    {
        private readonly ILogger<FelhanteringsController> _logger;

        public FelhanteringsController(ILogger<FelhanteringsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Vi loggar på informations nivå");
            _logger.LogError("Vi loggar på error nivå");
            _logger.LogCritical("Vi loggar på kritisk nivå");
            return "Vi har loggat lite fel";
        }
    }
}
