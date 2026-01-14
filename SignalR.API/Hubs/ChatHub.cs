using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;

public class ChatHub : Hub
{

    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    public async Task Register(string userName)
    {
        await Clients.All.SendAsync(
            "UsersUpdated",
            userName
        );
    }

}
