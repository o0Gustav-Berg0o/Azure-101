using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

[ApiController]
[Route("api/notify")]
public class NotifyController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hub;

    public NotifyController(IHubContext<ChatHub> hub)
    {
        _hub = hub;
    }

    [HttpPost]
    public async Task Send(string message)
    {
        await _hub.Clients.All.SendAsync("ReceiveMessage", message);
    }
}
