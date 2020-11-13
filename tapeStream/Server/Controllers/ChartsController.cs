using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using tapeStream.Server.Managers;
using static tapeStream.Shared.Data.TDAChart;

namespace tapeStream.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        // GET: api/Charts
        [HttpGet]
        public async Task<Bollinger> getBollingerBands()
        {
            var x = await TDAChartManager.GetBollingerBands();
            return x;
        }

    }
}
