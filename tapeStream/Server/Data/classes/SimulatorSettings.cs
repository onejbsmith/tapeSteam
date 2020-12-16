using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tapeStream.Server.Data.classes
{
    public class SimulatorSettings
    {
        public bool? isSimulated { get; set; }
        public string runDate { get; set; }
        public DateTime runDateDate { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public DateTime currentSimulatedTime { get; set; }

        private DateTime myVar;

        public DateTime runStartDateTime
        {
            get { return  runDateDate.Add(startTime.TimeOfDay); }
        }

        public bool rebuildAllRatioFrames { get; internal set; }
        public int clockDelay { get; internal set; } = 300;
    }
}
