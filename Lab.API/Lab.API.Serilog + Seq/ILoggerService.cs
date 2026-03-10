namespace Lab.API.Serilog___Seq
{
    public interface ILoggingService<T>
    {
        void LogInformation(string message, object? data = null);
    }

    // 建立 LoggingService<T> 打包 ILogger<T> , 以利寫入資料庫欄位
    public class LoggingService<T> : ILoggingService<T>
    {
        private readonly ILogger<T> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingService(ILogger<T> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public void LogInformation(string message, object? data = null)
        {
            // 自動抓取當前 HTTP 的資訊
            var httpContext = _httpContextAccessor.HttpContext;

            // 創建新物件 MyProblemDetails , 把欄位設定
            var details = new MyProblemDetails
            {
                Title = "LogInfo",
                Status = 200,
                Detail = message,
                Instance = httpContext?.Request.Path,
                TraceId = httpContext?.TraceIdentifier,
                ControllerName = httpContext?.GetRouteData()?.Values["controller"]?.ToString(),
                ActionName = httpContext?.GetRouteData()?.Values["action"]?.ToString(),
            };

            // 加 @ 解構子 , 讓資訊展開
            _logger.LogInformation("{Message} {@result} {@structuredData}", message, details, data);
        }
    }
}
