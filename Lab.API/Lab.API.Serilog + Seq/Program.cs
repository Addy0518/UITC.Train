using System.Collections.ObjectModel;
using System.Data;
using Lab.API.Serilog___Seq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

// 取得連線字串
var configuration = builder.Configuration;
var serilogConnectionString =
    configuration["ConnectionStrings:SerilogConnectionString"]?.ToString() ?? string.Empty;

// 保留 Log 資料表的 SourceContext 欄位
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

// 直截讀取組態檔 , 並加入 Seq 的位址
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
    .CreateLogger();
try
{
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // 註冊 UseSerilog
    builder.Host.UseSerilog();
    // 註冊 AddHttpContextAccessor 存取 Http 請求
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped(typeof(ILoggingService<>), typeof(LoggingService<>));

    var app = builder.Build();
    // 註冊自訂 Middleware
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
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
