using System.Collections.Generic;
using System.Linq;
using tapeStream.Server.Models;

namespace tapeStream.Server.Pages
{
    public partial class Database
    {

        tapeStreamDb_devContext db = new tapeStreamDb_devContext();


        private List<Asks> asks;

        private List<Bids> bids;
        private List<Buys> buys;
        private List<Sells> sells;

        private List<Marks> marks;

        protected override void OnInitialized()
        {

            buys = db.Buys.ToList();

            sells = db.Sells.ToList();
            bids = db.Bids.ToList();
            asks = db.Asks.ToList();
            marks = db.Marks.ToList();

            //var ask1 = new Asks()
            //{
            //    StreamId = 1,
            //    Price = 111.11m,
            //    Size = 111
            //};


            //var ask2 = new Asks()
            //{
            //    StreamId = 2,
            //    Price = 222.22m,
            //    Size = 222
            //};

            //var asks2 = new List<Asks>()
            //{ ask1, ask2 };

            //db.Asks.AddRange(asks2);

            //db.SaveChanges();


        }
    }
}
