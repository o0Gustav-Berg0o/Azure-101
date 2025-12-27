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
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] string message)
    {
        try
        {
            // TODO 1: Skapa ServiceBusClient
            var connectionString = _config["Azure:ServiceBus"];
            await using var client = new ServiceBusClient(connectionString);

            // TODO 2: Skapa sender för queue
            var sender = client.CreateSender("image-processing-queue");

            // TODO 3: Skicka meddelande
            var serviceBusMessage = new ServiceBusMessage(message);
            await sender.SendMessageAsync(serviceBusMessage);

            _logger.LogInformation($"Sent message: {message}");

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
    [HttpPost("receive")]
    public async Task<IActionResult> Receive()
    {
        try
        {
            // TODO 4: Skapa ServiceBusClient
            var connectionString = _config["Azure:ServiceBus"];
            await using var client = new ServiceBusClient(connectionString);

            // TODO 5: Skapa receiver för queue
            var receiver = client.CreateReceiver("image-processing-queue");

            // TODO 6: Ta emot meddelande (max 10 sekunder wait)
            var message = await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(10));

            if (message == null)
                return Ok(new { message = "No messages in queue" });

            var body = message.Body.ToString();

            // TODO 7: Complete meddelandet (ta bort från queue)
            await receiver.CompleteMessageAsync(message);

            _logger.LogInformation($"Received message: {body}");

            return Ok(new { message = body });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error receiving message");
            return StatusCode(500, "Error receiving message");
        }
    }
}