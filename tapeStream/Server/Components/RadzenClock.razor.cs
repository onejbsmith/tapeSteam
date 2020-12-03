using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Server.Components
{
    public partial class RadzenClock
    {
        private DateTime _now = DateTime.MaxValue;

        [Parameter]
        public DateTime date
        {
            get { return _now; }
            set
            {
                _now = value;
                hours = _now.Hour + _now.Minute / 60.0;

                minutes = _now.Minute * minorStep + _now.Second * 12 / 3600.0;
                seconds = _now.Second * minorStep;

                InvokeAsync(StateHasChanged);
            }
        }


        [Parameter]
        public double scale { get; set; } = 0.5;


        System.Threading.Timer timer;
        double hours;
        double minutes;
        double seconds;

        double minorStep = 12 / 60.0;

        double move = 338;
        double exercise = 2;
        double stand = 8;

        protected override void OnInitialized()
        {
            if (date == DateTime.MaxValue)
            {
                timer = new System.Threading.Timer(state =>
                {
                    date = DateTime.Now;

                    InvokeAsync(StateHasChanged);
                }, null, 0, 1000);
            }
        }

        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();
        }
    }
}
