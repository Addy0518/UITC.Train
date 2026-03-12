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
