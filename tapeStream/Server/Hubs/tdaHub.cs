using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace tdaStreamHub.Hubs
{
    public class TDAHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            if (Clients != null)
                await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendTopic(string topic, string contents)
        {
            if (Clients != null)
                await Clients.All.SendAsync(topic, topic, contents);
        }
    }
}
