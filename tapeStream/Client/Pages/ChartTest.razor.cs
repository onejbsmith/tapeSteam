#define UsingSignalHub
#define dev
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using tapeStream.Client.Data;
using tapeStream.Shared.Data;
using tapeStream.Shared;
using tapeStream.Shared.Services;
using MatBlazor;
using Threader = System.Threading;
using tapeStream.Client.Components;
using Microsoft.JSInterop;

namespace tapeStream.Client.Pages
{
    public partial class ChartTest
    {
        [Inject] BlazorTimer Timer { get; set; }
        [Inject] BookColumnsService bookColumnsService { get; set; }
        [Inject] ChartService chartService { get; set; }


    }
}
