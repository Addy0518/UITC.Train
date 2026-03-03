namespace Lab.API.Middleware;

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
