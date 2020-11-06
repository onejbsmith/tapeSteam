using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using tdaStreamHub.Data;
using tapeStream.Shared.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tdaStreamHub.Controllers
{
    [Route("api/[controller]")]
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

    }
}
