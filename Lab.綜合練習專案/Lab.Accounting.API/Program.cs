using Lab.Accounting.API.Infrastructures.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NSwag.Generation.Processors.Security;

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

        options.AddSecurity(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "輸入 Token ",
            }
        );

        options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
    });
    // 啟用 CORS 設定
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            "AllowVueApp",
            policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                ;
            }
        );
    });

    builder
        .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // 有錯誤就會顯示詳細原因
            options.IncludeErrorDetails = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                // 發行人
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),
                // 接收者
                ValidateAudience = false,
                ValidAudience = "JwtAuthDemo",
                // Token 的有效期間
                ValidateLifetime = true,
                // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                ValidateIssuerSigningKey = true,
                // key
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration.GetValue<string>("JwtSettings:SignKey")
                    )
                ),
            };
        });

    // 加入剛剛設定的錯誤處理 middleware
    builder.Services.AddExceptionHandler<InternalServerExceptionHandler>();
    builder.Services.AddProblemDetails();
    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddDiConfig();

    var app = builder.Build();
    app.UseHttpsRedirection();
    app.UseCors("AllowVueApp");

    app.UseOpenApi();
    app.UseSwaggerUI();

    // 加入 response 跟 request 讀取 middleware
    app.UseMiddleware<ResponseRequestMiddleware>();
    app.UseSerilogRequestLogging(opts =>
        opts.EnrichDiagnosticContext = SerilogConfig.EnrichFromRequest
    );

    app.UseExceptionHandler();

    app.UseAuthentication();
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
