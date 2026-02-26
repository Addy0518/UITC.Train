# Route 的學習心得



1.  慣例路由 , 系統在MVC建立時就會自動設定

```csharp

// 預設路由 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

```

2. 也可以自己設定

```csharp

// 可以自己在新增慣用的預設路由
app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{*article}",
    defaults: new { controller = "Blog", aciton = "Article" }
    );

```

3. 在program設定之後就不用再單獨設定路由 , MyDisplayRouteInfo是用來直接查看路由的套件

```csharp

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

```

```csharp

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

```

4. 路由後面的關鍵字盡量不要取系統會使用的,不然會衝突

```csharp

  // 像 page 這種是系統保留關鍵字,用在路由會讓系統衝突
  [Route("/articles/{page}")]
  public IActionResult ListArticles(int page)
  {
      return ControllerContext.MyDisplayRouteInfo(page);
  }

```

5. Get 可以加上id查看指定資料,也可以限制型態 

```csharp

 [HttpGet("{id}")]   // GET / api / Test / xyz
 public IActionResult GetProduct(string id)
 {
     return ControllerContext.MyDisplayRouteInfo(id);
 }

 // 路由可以限制參數的型態
 [HttpGet("int/{id:int}")] // GET / api / Test / int / 3
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

```

6. 還有像 Post , Put , Delete 都是Restful API , 都可以使用相同 URL 


7. 可以設定多個路由名稱 , 不侷限本來的 controller 或 action 的名稱

```csharp

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

}

```


8. IUrlHelper 拿到另一個動作的 URL

```csharp

public IActionResult Sourse()
{
    // 拿到 Destination 的 Url
    var url = Url.Action("Destination");
    return ControllerContext.MyDisplayRouteInfo(" ", $"url={url}");

}

// Home / Destination
public IActionResult Destination()
{
    return ControllerContext.MyDisplayRouteInfo();
}

```

9. 也可以自訂一個 URL 物件

```csharp

// Home/UrlGet
public IActionResult UrlGet()
{ 
    var url = Url.Action("Buy", "Product", new { id = 17, color = "red" });
    return Content(url);
}

```

10. MVC 指定action跳轉頁面

```csharp

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

```

11. Area 區域

```csharp

  <li class="nav-item">
      <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
  </li>
   
   <li class="nav-item">
     <a class="nav-link text-dark" asp-area="TestAll" asp-controller="Home" asp-action="Index">TestAll</a>
  </li>


    // 如果是不同區域的話 , 就要標註是哪一個Area
    [Area("TestAll")]
    public class HomeController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
    }
```