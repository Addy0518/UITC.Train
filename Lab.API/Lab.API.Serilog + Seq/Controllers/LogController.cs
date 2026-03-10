using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Lab.API.Serilog___Seq.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILoggingService<LogController> _looger;

        public LogController(ILoggingService<LogController> looger)
        {
            _looger = looger;
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    _looger.LogInformation("This is a LogInformation.");
        //    _looger.LogWarning("This is LogWarning");
        //    _looger.LogError("This is LogError");
        //    return new[] { "value1", "value2" };
        //}

        [HttpGet("fault")]
        public IActionResult GetFalut()
        {
            _looger.LogInformation("hello");
            return Ok();
        }
    }
}
