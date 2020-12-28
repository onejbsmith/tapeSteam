using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using tapeStream.Server.Data;
using tapeStream.Shared;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tapeStream.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PrintsPieChartController : ControllerBase
    {
        // GET: api/<PrintsPieChartController>
        [HttpGet]
        public async Task<double> GetPrintsGaugeScore(string symbol)
        {
            double it = await TDAPrintsManager.GetPrintsGaugeScore(symbol);
            return it;
        }

        // GET api/<PrintsPieChartController>/5
        [HttpGet("{id}")]
        [Route("api/[controller]/id")]
        public async Task<Dictionary<string, DataItem[]>> GetPrintsPies(int id, string symbol)
        {
            Dictionary<string, DataItem[]> it = await TDAPrintsManager.GetPrintsPies(id, symbol);
            return it;
        }

        // POST api/<PrintsPieChartController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PrintsPieChartController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PrintsPieChartController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
