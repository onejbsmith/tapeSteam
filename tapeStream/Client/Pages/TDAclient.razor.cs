using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Pages
{
    public partial class TDAclient
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
        private string topicInput;
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
                .WithUrl("https://localhost:44367/tdahub")
                .Build();

            /// Setup a callback for Receiving messages from hub
            /// "NASDAQ_BOOK", "TIMESALE_EQUITY", "CHART_EQUITY", "OPTION", "QUOTE","ACTIVES"
            hubConnection.On("NASDAQ_BOOK", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("TIMESALE_EQUITY", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("CHART_EQUITY", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("OPTION", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("QUOTE", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("ACTIVES_NYSE", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("ACTIVES_NASDAQ", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            hubConnection.On("ACTIVES_OPTIONS", (Action<string, string>)((topic, message) => { Receive(topic, message); }));
            /// Start a Connection to the hub
            /// 
            await hubConnection.StartAsync();
        }

        /// <summary>
        /// Process message received from Hub
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        private void Receive(string topic, string message)
        {
            ///
            var encodedMsg = $"{topic}: {message}";
            ///
            messages.Clear();
            messages.Add(encodedMsg);

            Task.Yield();
            Task.Delay(1).Wait();
            StateHasChanged();
        }

        /// <summary>
        /// Method for this client to Send a message to the hub
        /// </summary>
        /// <returns></returns>
        /// 
        Task Send() =>
            hubConnection.SendAsync("SendMessage", topicInput, messageInput);

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
