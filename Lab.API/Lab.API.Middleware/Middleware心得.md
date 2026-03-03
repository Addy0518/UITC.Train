# Middleware心得 

### Middleware 從接收 Request 到丟回 Response 的一整個模組

1. 一般用 Run , Use , Map 來自訂 Middleware , Run 是指 Middleware 中最末端的行為 , 不會收到參數 , 後面也不會執行 , 像一道牆

```csharp
app.Run(async context =>
{
    // 直接丟 Response回去 , 不接參數
    await context.Response.WriteAsync("Creat customize [Run] example");
});
```

2. Use 是設定邏輯來製作成一層 Middleware , 可以透過呼叫 next() 來決定要不要進入下一層

```csharp
// Use 執行順序會是 No1 => No2 => Creat customize [Run] example => No3 => No4
app.Use(
    async (context, next) =>
    {
        await context.Response.WriteAsync("Use No1\r\n");
        // 進入下一層
        await next.Invoke();
        await context.Response.WriteAsync("Use No4\r\n");
    }
);
app.Use(
    async (context, next) =>
    {
        await context.Response.WriteAsync("Use No2\r\n");
        await next.Invoke();
        await context.Response.WriteAsync("Use No3\r\n");
    }
);

// Run
app.Run(async context =>
{
    // 直接丟 Response回去 , 不接參數
    await context.Response.WriteAsync("Creat customize [Run] example");
});
```

3. 因為全部 Use 都在 Program 設定的話會很擁擠 , 所以把他們拉出來到一個 cs 檔去寫成靜態方法

```csharp
public static class CustomMiddlewareExtetion
{
    // 建立 Middleware 擴充方法 , 就可以在 Program 使用 app.UseCustom();
    public static void UseCustom(this IApplicationBuilder app)
    {
        app.UseMiddleware<CustomMiddleware>();
    }
}

public class CustomMiddleware
{
    // 注入 Use 需要用到的 Request 功能
    private readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // Httpcontext 就是被運送的資料 , InvokeAsync這個方法會決定要不要往下一個 Middleware 跑 (_next.Invoke(context);)
    public async Task InvokeAsync(HttpContext context)
    {
        await context.Response.WriteAsync("Use No1\r\n");
        await _next.Invoke(context);
        await context.Response.WriteAsync("Use No2\r\n");
    }
}
```
```csharp
// Use
app.UseCustom();

// Run
app.Run(async context =>
{
    // 直接丟 Response回去 , 不接參數
    await context.Response.WriteAsync("Creat customize [Run] example");
});
```

4. Map 是負責管制 Middleware 的管線 , 符合條件就執行 , 反之則不執行 , 也可以進行分支比對

```csharp
//  https://localhost:xxxx/Map1
app.Map(
    "/map1",
    mapApp =>
    {
        // 順序是 in => Second => out
        mapApp.Use(
            async (context, next) =>
            {
                await context.Response.WriteAsync("Second Middleware in. \r\n");
                await next.Invoke();
                await context.Response.WriteAsync("Second Middleware out. \r\n");
            }
        );
        mapApp.Run(async context =>
        {
            await context.Response.WriteAsync("Second. \r\n");
        });
    }
);

app.Map("/map2", Map2);
app.Map("/map3", Map3);

// 巢狀結構 , 要先執行 Map2 在執行 nextMap2
static void Map2(IApplicationBuilder app)
{
    app.Map(
        "/nextMap2",
        mapApp =>
        {
            mapApp.Use(
                async (context, next) =>
                {
                    await context.Response.WriteAsync("nextMap2 IN. \r\n");
                    await next.Invoke();
                    await context.Response.WriteAsync("nextMap2 out. \r\n");
                }
            );
            mapApp.Run(async context =>
            {
                await context.Response.WriteAsync("Second. \r\n");
            });
        }
    );
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map 1");
    });
}
static void Map3(IApplicationBuilder app)
{
    app.Run(async context =>
    {
        await context.Response.WriteAsync("Map 2");
    });
}
```

5. WebAPI 也有很多自帶的 Middleware 

```csharp
// 靜態檔案中介 : 會在指定的路徑中尋找對應的靜態檔案，並在接收到相應的 HTTP 請求時將它們發送到客戶端
app.UseStaticFiles(); 
// 路由相關處理
app.UseRouting();
// 授權使用者存取安全資源
app.UseAuthorization();
// Session 相關處理
app.UseSession();
```