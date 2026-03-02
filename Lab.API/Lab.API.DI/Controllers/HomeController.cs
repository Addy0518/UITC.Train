using System.Diagnostics;
using Lab.API.DI.Models;
using Lab.API.DI.Service;
using Microsoft.AspNetCore.Mvc;

namespace Lab.API.DI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISample _transient;
        private readonly ISample _scoped;
        private readonly ISample _singleton;

        public HomeController(
            ISampleTransient transient,
            ISampleScoped scoped,
            ISampleSingleton singleton
        )
        {
            _transient = transient;
            _scoped = scoped;
            _singleton = singleton;
        }

        public IActionResult Index()
        {
            // 4. 在呼叫就有了! 這樣之後要實作別的類別,只要有 IElectricalPlug 介面都行
            System.Console.WriteLine("使用吹風機插頭");
            IElectricalPlug hairDryerPlug = new HairDryerPlug();
            var socket = new Socket(hairDryerPlug);
            socket.SendPower();

            ViewBag.TransientId = _transient.Id;
            ViewBag.TransientHashCode = _transient.GetHashCode();

            ViewBag.ScopedId = _scoped.Id;
            ViewBag.ScopedHashCode = _scoped.GetHashCode();

            ViewBag.SingletonId = _singleton.Id;
            ViewBag.SingletonHashCode = _singleton.GetHashCode();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                }
            );
        }
    }
}
