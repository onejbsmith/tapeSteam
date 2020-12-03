using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Threading.Tasks;

namespace tapeStream.Server.Components
{
    public partial class RadzenClock
    {
        private DateTime _now = DateTime.Now;

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

                StateHasChanged();
            }
        }


        [Parameter]
        public double scale { get; set; } = 0.5;


        //System.Threading.Timer timer;
        double hours;
        double minutes;
        double seconds;

        double minorStep = 12 / 60.0;

        double move = 338;
        double exercise = 2;
        double stand = 8;

        protected override void OnInitialized()
        {
            //timer = new System.Threading.Timer(state =>
            //{
            //    var now = DateTime.Now;

            //    hours = now.Hour + now.Minute / 60.0;

            //    minutes = now.Minute * minorStep + now.Second * 12 / 3600.0;
            //    seconds = now.Second * minorStep;

            //    InvokeAsync(StateHasChanged);
            //}, null, 0, 1000);
        }

        public void Dispose()
        {
            //timer.Dispose();
        }
    }
}
