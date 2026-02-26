using Lab.API.Route.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Docs.Samples;
using System.Diagnostics;

namespace Lab.API.Route.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Sourse()
        {
            
            var url = Url.Action("Destination");
            return ControllerContext.MyDisplayRouteInfo(" ", $"url={url}");

        }

        // Home / Destination
        public IActionResult Destination()
        {
            return ControllerContext.MyDisplayRouteInfo();
        }

        // Home/UrlGet
        public IActionResult UrlGet()
        {
            var url = Url.Action("Buy", "Product", new { id = 17, color = "red" });
            return Content(url);
        }


        // Rest API 屬性路由 , HomeController 會比對跟慣例路由相近的
        [Route("")]
        [Route("Home")]
        [Route("Home/Index")]
        [Route("Home/Index/{id?}")]
        public IActionResult Index2(int? id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }

        [Route("Home/About")]
        [Route("Home/About/{id?}")]
        public IActionResult About(int? id)
        {
            return ControllerContext.MyDisplayRouteInfo(id);
        }

        // 像 page 這種是系統保留關鍵字,用在路由會讓系統衝突
        [Route("/articles/{page}")]
        public IActionResult ListArticles(int page)
        {
            return ControllerContext.MyDisplayRouteInfo(page);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
