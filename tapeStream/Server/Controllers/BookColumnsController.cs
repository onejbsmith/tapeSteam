using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tapeStream.Server.Data;
using tapeStream.Shared.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tapeStream.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class BookColumnsController : ControllerBase
    {
        // GET: api/<BookColumnsController>
        [HttpGet]
        public async Task<Dictionary<string, BookDataItem[]>> getBookColumnsData()
        {
            return await TDABookManager.getBookColumnsData();
        }

        [HttpGet("{id}")]
        public async Task<Dictionary<string, BookDataItem[]>> getPrintsLineChartData(int id)
        {
            return await TDABookManager.getBookColumnsData(id);
        }


        [Route("getAverages/{seconds}")]
        public async Task<AverageSizes> getAverages(int seconds)
        {
            return await TDABookManager.getAverages(seconds);
        }

        [Route("getLtAverages/{seconds}")]
        public async Task<AverageSizes> getLtAverages(int seconds)
        {
            return await TDABookManager.getLtAverages(seconds);
        }

        [Route("getLtRatios/{seconds}")]
        public async Task<AverageSizes> getLtRatios(int seconds)
        {
            return await TDABookManager.getLtRatios(seconds);
        }

        [Route("getListLtRatios/{seconds}/{last}")]
        public async Task<List<RatioFrame>> getListLtRatios(int seconds, int last)
        {
            return await TDABookManager.getListLtRatios(seconds, last);
        }

        [Route("getRatioFrames/{seconds}/{last}")]
        public async Task<List<RatioFrame>> getRatioFrames(int seconds, int last)
        {
            return await TDABookManager.getRatioFrames(seconds, last);
        }
        [Route("getRatioFramesCSV/{seconds}/{last}")]
        public async Task<string> getRatioFramesCsv(int seconds, int last)
        {
            return await TDABookManager.getRatioFramesCSV(seconds, last);
        }
    }
}
