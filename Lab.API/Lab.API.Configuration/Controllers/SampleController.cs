using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Lab.API.Configuration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        private readonly IConfiguration _configuration;

        // 新增一個剛剛創建的類別
        private readonly StrongholdInfoOptions _Info;

        // 依賴 StrongholdInfoOptions 介面 , 用 IOptions 取得內容
        public SampleController(
            IConfiguration configuration,
            IOptions<StrongholdInfoOptions> options
        )
        {
            _configuration = configuration;
            _Info = options.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var Api1 = _configuration["ApiSettings:ApiOne"];
            var Api2 = _configuration["ApiSettings:ApiTwo"];

            var constr = _configuration.GetConnectionString("DefaultConnection");

            return Ok(
                new
                {
                    constr,
                    Api1,
                    Api2,
                }
            );
        }

        [HttpGet("IOption")]
        public Object GetInfo()
        {
            return _Info;
        }
    }
}
