# Filter心得 

### 跟 Middleware 很像 , 不過他是管 Action 的執行前後的處理 , 總共有五種

```csharp
// 1. Authorization 的優先級最高 , 驗證 Middleware 進來的 Request 合不合規 , 不合就跳過 , 他只會一開始驗證不會驗證出去的 Response
// 繼承非同步的 AuthorizationFilter 實作方法 OnAuthorizationAsync
// 再繼承以前 MVC 註冊用的 [Attribute] , 待會 Action 就可以直些寫標籤 , 下面的類別都以此類推
public class AuthorizationFilter : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 用 GetType 看當下是什麼 Filter
        await context.HttpContext.Response.WriteAsync($"{GetType().Name} in. \r\n");
    }
}

// 2. Resource 第 2 優先 , 在 ModelBinding 以前執行
public class ResourceFilter : Attribute, IAsyncResourceFilter
{
    public async Task OnResourceExecutionAsync(
        ResourceExecutingContext context,
        ResourceExecutionDelegate next
    )
    {
        await context.HttpContext.Response.WriteAsync($"{GetType().Name} in. \r\n");

        await next();

        await context.HttpContext.Response.WriteAsync($"{GetType().Name} out. \r\n");
    }
}

// 3. Action 最常用 , Resource 經過 ModelBinding 來到這裡
public class ActionFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        await context.HttpContext.Response.WriteAsync($"{GetType().Name} in. \r\n");

        await next();

        await context.HttpContext.Response.WriteAsync($"{GetType().Name} out. \r\n");
    }
}

// Exception 處理當 Action 拋出錯誤時的情況
public class ExceptionFilter : Attribute, IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        context.HttpContext.Response.WriteAsync($"{GetType().Name} in. \r\n");
        return Task.CompletedTask;
    }
}

// Result 就是正常 Action 拋出的結果 , 再傳給 Middleware
public class ResultFilter : Attribute, IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next
    )
    {
        await context.HttpContext.Response.WriteAsync($"{GetType().Name} in. \r\n");

        await next();

        await context.HttpContext.Response.WriteAsync($"{GetType().Name} out. \r\n");
    }
}
```