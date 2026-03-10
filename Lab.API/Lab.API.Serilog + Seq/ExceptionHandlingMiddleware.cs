using Microsoft.AspNetCore.Mvc.Controllers;

namespace Lab.API.Serilog___Seq
{
    public class ExceptionHandlingMiddleware
    {
        // 做一個 middleware 記錄全域 API
        private readonly RequestDelegate _next; // middleware 往下執行的程式碼
        private readonly ILogger<ExceptionHandlingMiddleware> _logger; // ILogger 紀錄

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 1. 請求一開始進來的時候紀錄他是誰 ( traceId ) 跟要去哪個 controller
                LogRequest(context);
                // 2. 繼續往下走
                await _next(context);
            }
            catch (Exception ex)
            {
                // 有錯誤就拋錯誤
                await HandleExceptionAsync(context, ex);
            }
        }

        private void LogRequest(HttpContext context)
        {
            // 取得該次請求的身分 ( traceId )
            var traceId = context.TraceIdentifier;
            // 待會用來抓 controller 跟 action 名稱的
            string? controllerName = null;
            string? actionName = null;
            // 抓到路由裡的資訊
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                // ControllerActionDescriptor 是抓 Controller 的專屬標籤 , 這樣才能抓到 ControllerName 跟 ActionName
                var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (actionDescriptor != null)
                {
                    controllerName = actionDescriptor.ControllerName;
                    actionName = actionDescriptor.ActionName;
                }
            }
            // 格式化輸出給 Seq
            _logger.LogInformation(
                "Incoming Request: TraceId={TraceId}, Controller={ControllerName}, Action={ActionName}",
                traceId,
                controllerName,
                actionName
            );
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 一樣拿到 ID
            var traceId = context.TraceIdentifier;

            // 一樣取得 Controller , Action 資訊
            var endpoint = context.GetEndpoint();
            var actionDescriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            string? controllerName = actionDescriptor?.ControllerName;
            string? actionName = actionDescriptor?.ActionName;

            // 抓到錯誤回傳格式
            var response = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "伺服器發生錯誤",
                Detail = exception.Message,
                Instance = context.Request.Path,
                Extensions = { ["traceId"] = traceId },
            };

            // 建立一個匿名物件，把所有想在 Seq 篩選的欄位都塞進去
            var result = new
            {
                Status = response.Status,
                Title = response.Title,
                Detail = response.Detail,
                Instance = response.Instance,
                ControllerName = controllerName,
                ActionName = actionName,
                TraceId = traceId,
            };

            // 統整收到的錯誤訊息的結構 , 判斷狀態然後回傳給 Seq , 這樣在 Seq 就能展開查看
            if (response.Status >= 400 && response.Status < 500)
                _logger.LogWarning("警告 {@result}", result);
            if (response.Status >= 500)
                _logger.LogError(exception, "錯誤: {@result}", result);
        }
    }
}
