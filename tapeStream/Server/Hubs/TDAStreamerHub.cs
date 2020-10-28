using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BlazorSignalRApp.Server.Hubs
{
    public class TDAStreamerHub : Hub
    {
        public async Task SendMessage(string topic, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", "TDA-" + topic, message);
        }
    }
}
