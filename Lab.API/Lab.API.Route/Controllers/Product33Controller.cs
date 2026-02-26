using Lab.API.Route.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Docs.Samples;

namespace Lab.API.Route.Controllers
{
    public class Product33Controller : Controller
    {
            // 這樣寫都會符合預設路由 {controller=Home}/{action=Index}/{id?} 
            public IActionResult Edit(int id)
            { 
               return ControllerContext.MyDisplayRouteInfo(id);

            }
            // Edit(int id) 負責顯示表單 , Edit(int id, Product product) 負責處理
            [HttpPost]
            public IActionResult Edit(int id, Product product)
            {
                return ControllerContext.MyDisplayRouteInfo(id, product.Name);
          
        }


        [HttpGet("/products3")]
        public IActionResult ListProducts()
        {
            return ControllerContext.MyDisplayRouteInfo();
        }

        [HttpPost("/products3")]
        public IActionResult CreateProduct(Product myProduct)
        {
            return ControllerContext.MyDisplayRouteInfo(myProduct.Name);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // 驗證
        public IActionResult EditProduct(int id, Product product)
        {
            if (ModelState.IsValid)
            {  
                // 用 ViewData 傳更新成功的訊息 
                ViewData["Message"] = $"Successful edit of customer {id}";
                // RedirectToAction就是傳統跳轉 View 的方法
                return RedirectToAction("Index");
            }
            return View(product);
        }

    }
}
