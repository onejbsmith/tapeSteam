using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tdaStreamHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class TimeSalesController : ControllerBase
    {
        // GET: api/<TimeSalesController>
        [HttpOptions]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<TimeSalesController>/5
        //[Produces("application/json")]
        [HttpGet("{id}")]
        [Route("api/[controller]/id")]
        public string Get(int id)
        {
            return $"value for {id}";
        }

    }
}
