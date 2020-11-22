using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tapeStream.Server.Managers;
using static tapeStream.Shared.Data.TDAChart;

namespace tapeStream.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
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

        [HttpGet("{id}")]
        public async Task<Chart_Content> getLastCandle(int id)
        {
            var x = TDAChartManager.getLastCandle();
            return x;
        }

        [Route("getSvcDateTime")]
        public async Task<DateTime> getSvcDateTime()
        {
            return TDAChartManager.getSvcDateTime();
        }

    }
}
