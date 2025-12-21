using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MetodIMolnet
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("Processing request...");

            // Get query param
            string name = req.Query["name"];

            // Read body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            return new OkObjectResult($"Query name: {name}, Body: {requestBody}");
        }


    }
}
