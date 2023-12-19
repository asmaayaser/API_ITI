using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindController : ControllerBase
    {
        //
        //route data => query string
        [HttpGet("{id:int}/{name:alpha}")]
        public IActionResult Get1(int id ,string name)
        {
            return Ok();
        }
        [HttpPost]
        public IActionResult Add(Employee  emp)
        {
            return Ok();
        }
        [HttpGet("{name}/{address}/{salary}")]
        public IActionResult Get2([FromRoute]Employee emp)
        {

            return Ok();
        }

        [HttpPost("body")]
        public IActionResult Post2([FromBody]string name)
        {
            return Ok();
        }


    }
}
