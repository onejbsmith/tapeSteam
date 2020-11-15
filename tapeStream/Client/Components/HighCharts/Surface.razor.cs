using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tapeStream.Shared.Data;

namespace tapeStream.Client.Components.HighCharts
{
    public partial class  Surface
    {
        public Dictionary<string, BookDataItem[]> bookColData
        {
            get { return _bookColData; }
            set
            {
                _bookColData = value;
            }
        }
        Dictionary<string, BookDataItem[]> _bookColData;
    }
}
