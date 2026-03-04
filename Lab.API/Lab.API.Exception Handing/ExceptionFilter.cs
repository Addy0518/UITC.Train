using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab.API.Exception_Handing
{
    // 建立 ExceptionFilter
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            // 非同步拿到錯誤訊息
            context.HttpContext.Response.WriteAsync(
                $"{GetType().Name} catch exception. Message: {context.Exception.Message}"
            );
            // 回傳已完成的工作
            return Task.CompletedTask;
        }
    }
}
