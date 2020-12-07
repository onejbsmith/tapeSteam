using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Client.Shared
{

    using System;
    using System.Threading.Tasks;
    using System.Timers;
    using tapeStream.Client.Data;

    public partial class NavMenu
    {
        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        string hubStatus = "";
        string hubStatusMessage = "tapeStream Hub Connection Status";

        public string timeOfDay;

        public  DateTime _clockDateTime;

        Timer timerClock = new Timer(1000);



        protected override async Task OnInitializedAsync()
        {
            hubStatus = TDAStreamerData.hubStatus;

            TDAStreamerData.OnHubStatusChanged += HubStatusChanged;
            await Task.CompletedTask;

            timerClock.Elapsed += async (sender, e) => await timerClock_Elapsed(sender, e);
            timerClock.Start();
        }

        public void HubStatusChanged()
        {
            hubStatus = TDAStreamerData.hubStatus;
            hubStatusMessage =  TDAStreamerData.hubStatusMessage;

            StateChangedAsync();
        }

        private async Task StateChangedAsync()
        {
            await InvokeAsync(() => StateHasChanged());
        }

        private async Task timerClock_Elapsed(object sender, ElapsedEventArgs e)
        {
            _clockDateTime = DateTime.Now;
            timeOfDay = DateTime.Now.ToString("dddd, MM d, yy   h:mm:ss tt");
            StateHasChanged();
            await Task.CompletedTask;
        }

        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

    }
}

