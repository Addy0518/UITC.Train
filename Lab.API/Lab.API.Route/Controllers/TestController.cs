using Microsoft.AspNetCore.Mvc;
using Microsoft.Docs.Samples;

namespace Lab.API.Route.Controllers
{
    [Route("Store")]
    [Route("[controller]")]
    [ApiController]
    public class TestController : Controller
    {

        // 同時符合 Test/Buy , Store/Buy , Test/Checkout , Store/Checkout
        [HttpPost("Buy")]
        [HttpPost("Checkout")]  
        public IActionResult Buy()
        {
            return ControllerContext.MyDisplayRouteInfo();
        }

        [HttpGet("{id}")]   // GET /api/Test/xyz
        public IActionResult GetProduct(string id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }

        // 路由可以限制變數的型態
        [HttpGet("int/{id:int}")] // GET /api/Test/int/3
        public IActionResult GetIntProduct(int id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }

        // 這裡路由沒限制,但是參數型態是 int 
        // 這樣當今天丟其他型態的參數就會報錯誤 所以在路由限制我覺得比較方便

        [HttpGet("int2/{id}")]  // GET / api / Test / int2 / abc
        public IActionResult GetInt2Product(int id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }
    }
}
