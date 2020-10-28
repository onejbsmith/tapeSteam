using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Pages
{
    public partial class Chat
    {
        /// <summary>
        /// 
        /// </summary>
        private HubConnection hubConnection;

        /// <summary>
        /// 
        /// </summary>
        private List<string> messages = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        private string userInput;
        private string messageInput;

        /// <summary>
        /// Runs once, when this page is first loaded
        /// </summary>
        /// <returns></returns>
        /// 
        protected override async Task OnInitializedAsync()
        {
            /// Setup a hub Url
            /// 
            hubConnection = new HubConnectionBuilder()
                //.WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
                .WithUrl("https://localhost:44367/chathub")
                .Build();

            /// Setup a callback for Receiving messages from hub
            /// 
            hubConnection.On("ReceiveMessage", (Action<string, string>)((user, message) =>
            {
                Receive(user, message);
            }));

            /// Start a Connection to the hub
            /// 
            await hubConnection.StartAsync();
        }

        /// <summary>
        /// Process message received from Hub
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        private void Receive(string user, string message)
        {
            ///
            var encodedMsg = $"{user}: {message}";
            ///
            messages.Add(encodedMsg);

            StateHasChanged();
        }

        /// <summary>
        /// Method for this client to Send a message to the hub
        /// </summary>
        /// <returns></returns>
        /// 
        Task Send() =>
            hubConnection.SendAsync("SendMessage", userInput, messageInput);

        /// <summary>
        /// Method to test if hub connection is alive
        /// </summary>
        /// 
        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;

        /// <summary>
        /// Method that will shut down hub if this page is closed
        /// </summary>
        /// 
        public void Dispose()
        {
            _ = hubConnection.DisposeAsync();
        }
    }
}
