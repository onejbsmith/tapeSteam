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
    public class PrintsLineChartController : ControllerBase
    {
        // GET: api/<PrintsLineChartController>
        [HttpGet]
        /// 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PrintsLineChartController>/5
        /// <summary>
        /// For the Arc Gauge single number
        /// </summary>
        /// <param name="id">
        /// Called from service
        /// </param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public  async Task<Dictionary<string, DataItem[]>> getPrintsLineChartData(int id)
        {
            return await TDAPrintsManager.getPrintsLineChartData(id);
        }

        // POST api/<PrintsLineChartController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PrintsLineChartController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PrintsLineChartController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
