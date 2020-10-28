using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tapeStream.Shared;
using static tapeStream.Shared.CONSTANTS;

namespace tapeStream.Data
{
    public class TDAPrints
    {
        public static DataItem[] newData = new DataItem[] {
            new DataItem { Quarter = "1", Revenue = 1 },
            new DataItem { Quarter = "2", Revenue = 2 },
            new DataItem { Quarter = "3", Revenue = 3 },
            new DataItem { Quarter = "4", Revenue = 4 },
            new DataItem { Quarter = "2", Revenue = 2 }
        };

        public static Dictionary<int, DataItem[]> dictPies
            = new Dictionary<int, DataItem[]>() { { 5, newData } };
    }

}
