using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using tdaStreamHub.Data;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tdaStreamHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookColumnsController : ControllerBase
    {
        // GET: api/<BookColumnsController>
        [HttpGet]
        public async Task<Dictionary<string, BookDataItem[]>> Get()
        {
            return await TDABookManager.getBookColumnsData();
        }

        // GET api/<BookColumnsController>/5
        /// <summary>
        /// For the T&S Lines Chart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BookColumnsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<BookColumnsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookColumnsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
