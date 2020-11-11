using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace tapeStream.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            if (Clients != null)
                await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}