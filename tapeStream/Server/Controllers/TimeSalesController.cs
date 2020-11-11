using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tapeStream.Server.Controllers
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
