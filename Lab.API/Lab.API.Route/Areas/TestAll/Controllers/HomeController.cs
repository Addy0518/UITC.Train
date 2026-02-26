using Microsoft.AspNetCore.Mvc;

namespace Lab.API.Route.Areas.TestAll.Controllers
{
    [Area("TestAll")]
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
}
