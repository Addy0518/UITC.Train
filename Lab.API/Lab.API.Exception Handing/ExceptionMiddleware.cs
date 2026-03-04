namespace Lab.API.Exception_Handing
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                // 當Middleware往下一步走時 ,拿到委派傳輸的訊息 , 有錯誤就 catch
                await _next(context);
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync($"{GetType().Name} 抓到錯誤 : {ex.Message}");
            }
        }
    }
}
