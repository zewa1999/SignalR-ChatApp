using Microsoft.AspNetCore.SignalR;

namespace BlazorServer.Hubs;

public class ChatHub : Hub
{
    public Task SendMessage(string user, string message)
    {
        return Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public Task SendMessageToCaller(string user, string message)
    {
        return Clients.Caller.SendAsync("ReceiveMessage", user, message);
    }
}
