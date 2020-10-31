using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using tdaStreamHub.Data;
using tapeStream.Shared.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tdaStreamHub.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class BookPieChartsController : ControllerBase
    {
        // GET: api/<BookPieChartsController>
        [HttpOptions]
        [HttpGet]
        public async Task<Dictionary<string, List<BookDataItem>>> getBookPiesData()
        {
            return await TDABookManager.getBookPiesData();
        }

        // GET api/<BookPieChartsController>/5
        /// <summary>
        /// Use this for the Composite Book Pie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<BookDataItem[]> getBookCompositePieData(int id)
        {
            return await TDABookManager.getBookCompositePieData();
        }

        // POST api/<BookPieChartsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BookPieChartsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookPieChartsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
