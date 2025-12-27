using Microsoft.AspNetCore.Mvc;
using Azure.Messaging.ServiceBus;

namespace SlutProv.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServiceBusController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly ILogger<ServiceBusController> _logger;

    public ServiceBusController(IConfiguration config, ILogger<ServiceBusController> logger)
    {
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Skicka ett meddelande till Service Bus
    /// </summary>
    /// <param name="request">Meddelandet att skicka</param>
    /// <remarks>
    /// Exempel:
    /// 
    ///     POST /api/servicebus/send
    ///     {
    ///         "message": "Hello from API"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Meddelande skickat framgångsrikt</response>
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Send([FromBody] string message)
    {
        try
        {
            // TODO 1: Skapa ServiceBusClient
         

            // TODO 2: Skapa sender för queue
          

            // TODO 3: Skicka meddelande
      

            _logger.LogInformation($"Sent message: message");

            return Ok(new { message = "Message sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, "Error sending message");
        }
    }

    /// <summary>
    /// Ta emot ett meddelande från Service Bus
    /// </summary>
    /// <remarks>
    /// Tar emot och tar bort ett meddelande från 'image-processing-queue'. Max väntetid 10 sekunder.
    /// </remarks>
    /// <response code="200">Meddelande mottaget eller kön är tom</response>
    [HttpPost("receive")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Receive()
    {
        try
        {
            // TODO 4: Skapa ServiceBusClient
           

            // TODO 5: Skapa receiver för queue
          

            // TODO 6: Ta emot meddelande (max 10 sekunder wait)
         
          

            // TODO 7: Complete meddelandet (ta bort från queue)
        

            _logger.LogInformation($"Received message: body");

            return Ok(new { message = "body" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving message");
            return StatusCode(500, "Error receiving message");
        }
    }
}