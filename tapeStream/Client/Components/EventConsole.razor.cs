using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Components
{
    public partial class EventConsole
    {

       public static Blazored.LocalStorage.ISyncLocalStorageService localStorage { get; set; }

        class Message
        {
            public DateTime Date { get; set; }
            public string Text { get; set; }
        }

        ElementReference console;

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attributes { get; set; }
        IList<Message> messages = new List<Message>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                //await JSRuntime.InvokeVoidAsync("scrollToBottom", console);
            }
        }

        void OnClearClick()
        {
            Clear();
        }

        public void Init()
        {
            if (localStorage.ContainKey("messages"))
                messages = localStorage.GetItem<List<Message>>("messages");
        }

        public void Clear()
        {
            messages.Clear();

            StateHasChanged();
        }

        public void Log(string message)
        {
            messages.Insert(0, new Message { Date = DateTime.Now, Text = message });
            localStorage.SetItem("messages", messages.Take(1000));
            StateHasChanged();
        }
    }
}
