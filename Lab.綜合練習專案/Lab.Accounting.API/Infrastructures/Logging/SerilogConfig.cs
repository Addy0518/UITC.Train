using Serilog.Exceptions;

namespace Lab.API.TODO.Infrastructures.Logging
{
    public class SerilogConfig
    {
        public static void AddSerilLog()
        {
            // 全域設定
            /*  🔔new CompactJsonFormatter()
             *  由於 Log 的欄位很多，使用 Console Sink 會比較看不出來，改用 Serilog.Formatting.Compact 來記錄 JSON 格式的 Log 訊息會清楚很多！
             */
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information() // 設定最小Log輸出
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) // 設定 Microsoft.AspNetCore 訊息為 Warning 為最小輸出
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning) // 設定 Microsoft.EntityFrameworkCore 訊息為 Warning 為最小輸出
                .Enrich.FromLogContext() // 可以增加Log輸出欄位 https://www.cnblogs.com/wd4j/p/15043489.html
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("Application", "UITC.Todo")
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:yyyy/MM/dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{RequestBody}{ResponseBody}{NewLine}{Exception}"
                );

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        public static async void EnrichFromRequest(
            IDiagnosticContext diagnosticContext,
            HttpContext httpContext
        )
        {
            var request = httpContext.Request;
            var requestBody = httpContext.Items["RequestBody"]?.ToString() ?? string.Empty;
            diagnosticContext.Set("RequestBody", requestBody);

            var responsebody = httpContext.Items["ResponseBody"]?.ToString() ?? string.Empty;
            diagnosticContext.Set("ResponseBody", responsebody);

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Scheme", request.Scheme);
            diagnosticContext.Set("Headers", request.Headers);

            string ip =
                request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext?.Connection.RemoteIpAddress?.ToString()
                ?? "Unknown";

            diagnosticContext.Set("RemoteIp", ip);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }
    }
}
