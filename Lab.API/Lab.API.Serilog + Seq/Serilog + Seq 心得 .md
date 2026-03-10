# Serilog + Seq 心得

1. 安裝套件 Serilog.AspNetCore , 它裡面已經有大部分 Serilog 會用到的套件

2. 在 Program 設定 Serilog

```csharp

// 設定 Serilog
// LoggerConfiguration 開始建立 Logger 的配置
Log.Logger = new LoggerConfiguration()
    // 預設 Log 等級是 Information , 基本上所有訊息都會顯示
    .MinimumLevel.Information()
    // 覆寫 Log Category 為 Microsoft.AspNetCore 的最小 Level Warning
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    // 添加 Serilog 當下的運作情境
    .Enrich.FromLogContext()
    // FromLogContext 是通知要加入欄位 , WriteTo.Console 再確實把欄位加入
    .WriteTo.Console(
    //outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:l}{NewLine}{Properties}{NewLine}{Exception}"
    )
    // 輸出至檔案  (log/log-yyyyMMdd.txt) , 每天更換一個記錄檔
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


// 或者直接讀取 appsetting
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();


```

3. try - catch 把 Program 包起來 , 這樣能捕捉到啟動時的錯誤

```csharp
try
{
    Log.Information("Starting web host");

    // 本來 Program 的程式碼放在這

    // 註冊 UseSerilog
    builder.Host.UseSerilog();

   
    return 0;
}
catch (Exception ex)
{
    // 紀錄未捕捉的 ex
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    // 把剩下的 Log 寫到 Sinks
    Log.CloseAndFlush();
}
```

4. 在 appsetting 設定 Serilog

```csharp 
{
  "Serilog": {
    // MinimumLevel 紀錄等級
    "MinimumLevel": {
      // 預設是給資訊
      "Default": "Information",
      // 類別設定為 warning
      "Override": {
        "Microsoft.AspNetCore": "Warning"
      }
    },
    // 增加回報資訊的豐富度
    "Enrich": [ "FromLogContext" ],
    // 記錄寫到 Console 面板 
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "./logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
```

5. 可以在 Middleware 設定 Request 進來時的訊息

```csharp
 app.UseSerilogRequestLogging(options =>
 {
     // 如果要自訂訊息的範本格式，可以修改這裡，但修改後並不會影響結構化記錄的屬性
     options.MessageTemplate = "Handled {RequestPath}";

     // 預設輸出的紀錄等級為 Information，你可以在此修改記錄等級
     //options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Debug;

     // 從 httpContext 取得 HttpContext 下所有可以取得的資訊
     options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
     {
         diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
         diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
         //diagnosticContext.Set("UserID", httpContext.User.Identity?.Name);
     };
 });
```

像這樣

```
09:30:35 INF] Starting web host
[09:30:35 INF] Now listening on: https://localhost:7240
[09:30:35 INF] Now listening on: http://localhost:5142
[09:30:35 INF] Application started. Press Ctrl+C to shut down.
[09:30:35 INF] Hosting environment: Development
[09:30:35 INF] Content root path: C:\Users\andychen\Desktop\UITC.Train\Lab.API\Lab.API.Serilog + Seq
[09:30:36 INF] Handled /swagger/index.html
[09:30:36 INF] Handled /swagger/v1/swagger.json
[09:30:39 INF] This is a LogInformation.
[09:30:39 WRN] This is LogWarning
[09:30:39 ERR] This is LogError
[09:30:39 INF] Handled /api/Log
```

6. Seq , 下載完註冊帳號 , 在 .Net 安裝套件 Serilog.Sinks.Seq , 新建一個 Middleware 用來抓訊息給 Seq

```csharp
// 直截讀取組態檔 , 並加入 Seq 的位址
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
```

```csharp
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
        throw;
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
```

7. 這樣在 Seq 就能看到完整的錯誤

```csharp
錯誤: {ActionName: null, ControllerName: null, Detail: 'Conflicting method/path combination "GET"'
ConnectionId
0HNJUFMNMA5D2
RequestId
0HNJUFMNMA5D2:00000007
RequestPath
/swagger/v1/swagger.json
result.ActionName
 // Controller 跟 Action 沒有是因為還沒進去就被抓到錯誤
result.ControllerName
 
result.Detail :
Conflicting method/path combination "GET api/Log" for actions - Lab.API.Serilog___Seq.Controllers.LogController.Get (Lab.API.Serilog + Seq),Lab.API.Serilog___Seq.Controllers.LogController.GetFalut (Lab.API.Serilog + Seq). Actions require a unique method/path combination for Swagger/OpenAPI 3.0. Use ConflictingActionsResolver as a workaround
result.Instance : /swagger/v1/swagger.json

result.Status : 500

result.Title : "logged on as contact AUTO-3af7-dadc93"

result.TraceId
0HNJUFMNMA5D2:00000007
SourceContext
Lab.API.Serilog___Seq.ExceptionHandlingMiddleware
```

8. 也可以利用語法像是 result.Status = 500 或是 result.ControllerName like "%Log%" 來搜尋

9. 將 Log 存在資料庫 , 先安裝套件 Serilog.Sinks.MSSqlServer

```csharp
"WriteTo": [
  { "Name": "Console" },
  {
    // 本來是存 Json 到檔案 , 改存到 sql
    "Name": "MSSqlServer",
    "Args": {
      "connectionString": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0",
      "schemaName": "dbo",
      "tableName": "Logs",
      // 這裡開啟它會自動建立一個表叫 Log , 欄位會自動對應 , 之後資料也會自動儲存
      "autoCreateSqlTable": true
    }
  }
  ]
```

10. 接下來是自訂 Log 資料表 , 改到 Program 設定

```csharp
// appsetting 改一下
 "ConnectionStrings": {
   // Serilog.Sinks.MSSqlServer 連接字串 改到這裡
   "SerilogConnectionString": "Data Source=localhost\\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0"
 },

 // 本來的設定搬到 program
 {
  "Name": "File",
  "Args": {
    "path": "./logs/log-.txt",
    "rollingInterval": "Day",
    "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
  }
}
```

```csharp
// 取得連線字串
var configuration = builder.Configuration;
var serilogConnectionString =
    configuration["ConnectionStrings:SerilogConnectionString"]?.ToString() ?? string.Empty;


// 保留 Log 資料表的 SourceContext 欄位
// 加入 ProblemDetails 的相關欄位 , 讓資訊更清楚
var columnOptions = new ColumnOptions
{
    AdditionalColumns = new Collection<SqlColumn>
    {
        new()
        {
            ColumnName = "SourceContext",
            DataType = SqlDbType.NVarChar,
            DataLength = 512,
            AllowNull = true,
            PropertyName = "SourceContext",
        },
        new()
        {
            ColumnName = "Title",
            DataType = SqlDbType.NVarChar,
            DataLength = 50,
            AllowNull = true,
            PropertyName = "result.Title",
        },
        new()
        {
            ColumnName = "Status",
            DataType = SqlDbType.Int,
            AllowNull = true,
            PropertyName = "result.Status",
        },
        new()
        {
            ColumnName = "Detail",
            DataType = SqlDbType.NVarChar,
            DataLength = 512,
            AllowNull = true,
            PropertyName = "result.Detail",
        },
        new()
        {
            ColumnName = "Instance",
            DataType = SqlDbType.NVarChar,
            DataLength = 512,
            AllowNull = true,
            PropertyName = "result.Instance",
        },
        new()
        {
            ColumnName = "TraceId",
            DataType = SqlDbType.NVarChar,
            DataLength = 128,
            AllowNull = true,
            PropertyName = "result.TraceId",
        },
        new()
        {
            ColumnName = "ControllerName",
            DataType = SqlDbType.NVarChar,
            DataLength = 256,
            AllowNull = true,
            PropertyName = "result.ControllerName",
        },
        new()
        {
            ColumnName = "ActionName",
            DataType = SqlDbType.NVarChar,
            DataLength = 256,
            AllowNull = true,
            PropertyName = "result.ActionName",
        },
        //new() { ColumnName = "Exception", DataType = SqlDbType.NVarChar, DataLength = -1, AllowNull = true } // -1 for max
    },
};

// 移除 MessageTemplate, Properties 這兩個欄位
columnOptions.Store.Remove(StandardColumn.Properties);
columnOptions.Store.Remove(StandardColumn.MessageTemplate);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Seq("http://localhost:5341")
    // 加入到資料庫的設定
    .WriteTo.MSSqlServer(
        connectionString: serilogConnectionString,
          // Sink 就是儲存的工具
        sinkOptions: new MSSqlServerSinkOptions
        {
            AutoCreateSqlTable = true,
            SchemaName = "dbo",
            TableName = "Logs",
        },
        columnOptions: columnOptions
    )
    .CreateLogger()
```

11. 然後創建一個資料表 Logs , 就會拿到紀錄了

```sql
CREATE TABLE [dbo].[Logs] (
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Message] [nvarchar](max) NULL,
    [Level] [nvarchar](max) NULL,
    [TimeStamp] [datetime] NULL,
    [Exception] [nvarchar](max) NULL,
    [Properties] [nvarchar](max) NULL,
    [SourceContext] [nvarchar](512) NULL,
    [Title] [nvarchar](50) NULL,
    [Status] [int] NULL,
    [Detail] [nvarchar](512) NULL,
    [Instance] [nvarchar](512) NULL,
    [TraceId] [nvarchar](128) NULL,
    [ControllerName] [nvarchar](256) NULL,
    [ActionName] [nvarchar](256) NULL,
 CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ( [Id] ASC )
); 
```

12. 輸出扁平化 , 把剛剛的紀錄資料每個欄位都能夠對應

```csharp
public class MyProblemDetails : ProblemDetails
{
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public string? TraceId { get; set; }
    public string? ControllerName { get; set; }
    public string? ActionName { get; set; }
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

// 最後註冊 
// 註冊 AddHttpContextAccessor 存取 Http 請求
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(typeof(ILoggingService<>), typeof(LoggingService<>));
```