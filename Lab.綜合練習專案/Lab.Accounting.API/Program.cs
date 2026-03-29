using Lab.Accounting.API.Infrastructures.Logging;

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
                Title = "綜合練習專案",
                Description = "記帳系統",
            };
        };
    });
    // 啟用 CORS 設定
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowVueApp", policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
    // 加入剛剛設定的錯誤處理 middleware
    builder.Services.AddExceptionHandler<InternalServerExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddDiConfig();

    var app = builder.Build();
    // app.UseHttpsRedirection();
    app.UseOpenApi();
    app.UseSwaggerUI();
    app.UseCors();
    // 加入 response 跟 request 讀取 middleware
    app.UseMiddleware<ResponseRequestMiddleware>();
    app.UseSerilogRequestLogging(opts =>
        opts.EnrichDiagnosticContext = SerilogConfig.EnrichFromRequest
    );
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseCors("AllowVueApp");
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
