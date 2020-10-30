using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using tdaStreamHub.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tdaStreamHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookPieChartsController : ControllerBase
    {
        // GET: api/<BookPieChartsController>
        [HttpGet]
        public async Task<Dictionary<int, BookDataItem[]>> Get()
        {
            return await TDABook.getBookPiesData();
        }

        // GET api/<BookPieChartsController>/5
        /// <summary>
        /// Use this for the Composite Book Pie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async  Task<BookDataItem[]> Get(int id)
        {
            return await TDABook.getBookCompositePieData();
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
