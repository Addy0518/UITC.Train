# TodoAPI 心得

1. 先安裝套件 Serilog , Sql.Cilent , Swagger 等等 , 然後放入連線

```csharp
"ConnectionStrings": {
  "TestConne": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0"
},
```

2. 開始做一些前置動作 , 把該設定的都做統一管理 , 不要全部都塞在 Program , 放在 ( Infrastructures 基礎設施 ) 資料夾    

```csharp
// 設定微軟打開文件方式 , 待會要讀取 xml 
 <GenerateDocumentationFile>True</GenerateDocumentationFile>

builder.Services.AddOpenApiDocument(options =>
{
    options.PostProcess = document =>
    {
        document.Info = new OpenApiInfo
        {
            // 可以放 Swagger 頁面標題跟描述
            Version = "v1",
            Title = "TODO 練習",
            Description = "練習 TODO 架構",
        };
    };
});
```


```csharp
// AddDIConfig
// 統一管理註冊 DI
public static void AddDIConfig(this IServiceCollection services)
{
    services.AddSingleton<TestConnection>();

    services.AddScoped<ITestService, TestService>();

    services.AddScoped<ITestRepository, TestRepository>();
}
```

```csharp
public class TestConnection(IConfiguration configuration)
{
    // 管理連線
    public SqlConnection CreateConnection() =>
        new SqlConnection(configuration.GetConnectionString("TestConne"));
}
```

```csharp
// 列舉狀態碼
public enum CodeStatus
{
    [Description("成功")]
    Success = 2000,

    [Description("Request驗證失敗")]
    RequestError = 4000,

    [Description("查無此資料")]
    NotFound = 4001,

    [Description("內部伺服器錯誤")]
    InternalException = 5000,
}
```

```csharp
// 統一回傳訊息 , 用泛型接收任何形態資料
public class ApiResponse<T>
{
    public CodeStatus CodeStatus { get; set; } = CodeStatus.Success;
    public string Message { get; set; } = string.Empty;
    public T ReturnData { get; set; } = default!;
}
```

```csharp
public static class ApiResponseHelper
{
    // 用靜態方法讓整個專案都能引用
    public static ApiResponse<T> Success<T>(T data, string message = "")
    {
        return new ApiResponse<T>
        {
            CodeStatus = CodeStatus.Success,
            ReturnData = data,
            Message = message,
        };
    }
}
```

```csharp
public class InsertRequest
{
    // 新增的請求格式
    /// <summary>
    /// 姓名
    /// </summary>
    [Display(Name = "姓名")]
    [Required(ErrorMessage = "{0} 必輸")]
    [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1}")]
    public string Name { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    [Display(Name = "角色")]
    public string Role { get; set; } = "User";

    /// <summary>
    /// 信箱
    /// </summary>
    [Display(Name = "信箱")]
    [Required(ErrorMessage = "{0} 必輸")]
    [EmailAddress(ErrorMessage = "信箱格式不正確")]
    public string Email { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    [Display(Name = "密碼")]
    [Required(ErrorMessage = "{0} 必輸")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度必須在 8 到 20 字元之間")]
    public string Password { get; set; }
}
```


```csharp
public class InsertRequest
{
    // 新增的請求格式
    /// <summary>
    /// 姓名
    /// </summary>
    [Display(Name = "姓名")]
    [Required(ErrorMessage = "{0} 必輸")]
    [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1}")]
    public string Name { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    [Display(Name = "角色")]
    public string Role { get; set; } = "User";

    /// <summary>
    /// 信箱
    /// </summary>
    [Display(Name = "信箱")]
    [Required(ErrorMessage = "{0} 必輸")]
    [EmailAddress(ErrorMessage = "信箱格式不正確")]
    public string Email { get; set; }

    /// <summary>
    /// 密碼
    /// </summary>
    [Display(Name = "密碼")]
    [Required(ErrorMessage = "{0} 必輸")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度必須在 8 到 20 字元之間")]
    public string Password { get; set; }
}
```

```csharp
 public class UpdateRequest
 {
     // 更新的請求格式
     /// <summary>
     /// ID
     /// </summary>
     [Display(Name = "ID")]
     [Required(ErrorMessage = "{0} 必輸")]
     public int Id { get; set; }

     /// <summary>
     /// 姓名
     /// </summary>
     [Display(Name = "姓名")]
     [Required(ErrorMessage = "{0} 必輸")]
     [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1}")]
     public string Name { get; set; }

     /// <summary>
     /// 信箱
     /// </summary>
     [Display(Name = "信箱")]
     [Required(ErrorMessage = "{0} 必輸")]
     [EmailAddress(ErrorMessage = "信箱格式不正確")]
     public string Email { get; set; }
 }
```

3. 開始做三層式 , 我這邊只放一個不然太多 , 並全部使用 XML 標註

```csharp
// repository
/// <summary>
/// 單筆取得 Users
/// </summary>
/// <param name="id">ID</param>
/// <returns>單個 Users </returns>
public async Task<User> GetUserAsync(int id)
{
    using var conn = connection.CreateConnection();
    {
        var sql = "Select * From [User] Where Id=@id";

        // new 一個新物件放 Id 進 Sql 裡
        return await conn.QuerySingleAsync<User>(sql, new { Id = id });
    }
}
```

```csharp
// service
/// <summary>
/// 單筆取得 Users
/// </summary>
/// <param name="id">ID</param>
/// <returns>單個 Users </returns>
public async Task<ApiResponse<User>> GetUserAsync(int id)
{
    var user = await repository.GetUserAsync(id);
    if (user == null)
    {
        return ApiResponseHelper.NotFound<User>();
    }

    return ApiResponseHelper.Success(user, "成功");
}
```

```csharp
// controller
/// <summary>
/// 單筆取得 Users
/// </summary>
/// <param name="id">ID</param>
/// <returns>單個 Users </returns>
/// <response code="200">回傳查到的物件</response>
/// <response code="404">如果物件是空的</response>
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<User>))]
public async Task<IActionResult> GetUserAsync(int id)
{
    return Ok(await testservice.GetUserAsync(id));
}



// 也可以給他範例請求
/// <summary>
/// 新增 User
/// </summary>
/// <param name="request">新增 User 請求</param>
/// <returns>新增 User</returns>
/// <remarks>
/// 範例請求 :
///
///     Post / Test
///     {
///        "name":"Andy",
///        "role":"User",
///        "email":"xxx@gmail.com",
///        "password":"xxxxxxxx"
///     }
///
/// </remarks>
/// <response code="200">回傳查到的物件</response>
[HttpPost]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<int>))]
```

4. 換流讀取 , 建立一個專門獨取 request 跟 response 的 Middleware , 因為 Request 進來的時候型態是 stream , C# 沒辦法讀取 , 所以要先轉換成 string 讀取完再丟回原本的 stram 丟回去

```csharp
public class RequestResponseLoggingMiddleware
{
    /// <summary>
    /// 注入請求委派這樣才可以往下一層 middleware
    /// </summary>
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// 換流讀取 , 把 stream 型態的 Request 跟 Response 換成 string , 讀完再塞回 stream
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        // 用做好的讀取 stream 方法
        string requestBodyPayload = await ReadRequestBody(context.Request);
        // 塞回 request 內部
        context.Items["RequestBody"] = requestBodyPayload;

        // response 也是一樣概念 , 先標記 Body
        var orginalresponse = context.Response.Body;

        // 給他複製一個新物件 stream , 塞進 body
        using (var responsebody = new MemoryStream())
        {
            context.Response.Body = responsebody;
            // 往下一層走
            await _next(context);
            // 再把原本的 stream 塞回去 , 真的進來被我們變成複製的 , 要出去時再把原本的塞回去的概念
            await responsebody.CopyToAsync(orginalresponse);
        }
    }

    /// <summary>
    /// 讀取 httpcontext 裡的請求資訊
    /// </summary>
    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        // EnableBuffering 是允許 request 可以被多次讀取
        request.EnableBuffering();
        // 因為 stream 讀取玩進度就會停在 stream 的結尾 , 所以要先記錄下來待會再把它 "倒帶" 回去
        var body = request.Body;
        // 根據請求的內容長度預留一個型態為 byte 的空間
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        // 叫系統把剛剛的內容塞到 buffer 裡 , 待會要讀取 (buffer,開始讀取位置,結束位置)
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        // 轉換成字串 , 也就是我們能看得懂的
        string requestbody = Encoding.UTF8.GetString(buffer);
        // 將位置調回最開始 0
        body.Seek(0, SeekOrigin.Begin);
        // 傳回剛剛解讀出來得資訊
        return $"{requestbody}";
    }
}
```

5. 設定 Serilog 跟 全域錯誤處理 ( 這裡我是直接照搬 )

```csharp
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
            .Enrich.WithProperty("Application", "UITC.Todo")
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
```

```csharp
public class InternalServerExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var title = exception.Message;
        var details = exception.ToString();

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Status = StatusCodes.Status500InternalServerError,
            Title = title,
            Detail = details,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}",
        };

        problemDetails.Extensions.TryAdd("requestId", httpContext.TraceIdentifier);
        problemDetails.Extensions.TryAdd("traceId", Activity.Current?.Id);

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponseHelper.InternalException(problemDetails),
            cancellationToken
        );

        return true;
    }
}
```

6. 最後在 Program 設定 , 並把 using 都移到 Global 全域 using

```csharp
var builder = WebApplication.CreateBuilder(args);

// 加入剛剛設定的 SerilogConfig
SerilogConfig.AddSerilLog();

// 預防啟動時出錯
try
{
    // 啟動環境提示
    Log.Information(
        $"Starting the application Environment: {builder.Environment.EnvironmentName} URL: {builder.Configuration["ASPNETCORE_URLS"]}"
    );
    // 第二次初始化 Serilog
    builder.Services.AddSerilog();
    builder.Services.AddControllers();
    builder.Services.AddOpenApiDocument(options =>
    {
        options.PostProcess = document =>
        {
            document.Info = new OpenApiInfo
            {
                // 可以放 Swagger 頁面標題跟描述
                Version = "v1",
                Title = "TODO 練習",
                Description = "練習 TODO 架構",
            };
        };
    });
    // 加入剛剛設定的錯誤處理 middleware
    builder.Services.AddExceptionHandler<InternalServerExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddDIConfig();

    var app = builder.Build();
    app.UseHttpsRedirection();
    app.UseOpenApi();
    app.UseSwaggerUI();
    // 加入 response 跟 request 讀取 middleware
    app.UseMiddleware<RequestResponseLoggingMiddleware>();
    app.UseSerilogRequestLogging(opts =>
        opts.EnrichDiagnosticContext = SerilogConfig.EnrichFromRequest
    );
    app.UseExceptionHandler();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

```


```csharp
global using System.ComponentModel.DataAnnotations;
global using Dapper;
global using Lab.API.TODO.Common.Enums;
global using Lab.API.TODO.Common.Extensions;
global using Lab.API.TODO.Common.Helpers;
global using Lab.API.TODO.Common.Requests;
global using Lab.API.TODO.Common.Responses;
global using Lab.API.TODO.Infrastructures.Data;
global using Lab.API.TODO.Infrastructures.Data.Entites;
global using Lab.API.TODO.Infrastructures.DependencyInjection;
global using Lab.API.TODO.Infrastructures.ExceptionHandler;
global using Lab.API.TODO.Infrastructures.Logging;
global using Lab.API.TODO.Repositories.Implements;
global using Lab.API.TODO.Repositories.Interfaces;
global using Lab.API.TODO.Services.Implements;
global using Lab.API.TODO.Services.Interfaces;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Mvc;
global using NSwag;
global using Serilog;

```