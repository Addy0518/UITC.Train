using System.Text.RegularExpressions;
using Lab.API.Exception_Handing;
using Lab.API.Exception_Handing.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 註冊 ExceptionFilter
builder.Services.AddControllers(
//(options) =>
//{
//    options.Filters.Add<ExceptionFilter>();
//}
);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TestContext>(options => options.UseSqlServer(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//// UseExceptionHandler 任何未處理的異常都會在這裡被抓到
//app.UseExceptionHandler(options =>
//{
//    // options.Run 處裡請求 , 但不呼叫下一個物件
//    options.Run(async context =>
//    {
//        // Http 狀態設為 500 , 回覆內容設定為Json
//        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//        context.Response.ContentType = "application/json";

//        // IExceptionHandlerFeature 允許存取原始異常 , 異常的詳細資訊會存入這個功能中
//        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
//        if (exceptionFeature is not null)
//        {
//            // 回復異常訊息
//            var error = new { message = "An unexpected error occurred" };
//            await context.Response.WriteAsJsonAsync(error);
//        }
//    });
//});

// 註冊自訂的 Middleware
//app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//拿到 HTTP 請求
app.Use(
    async (context, next) =>
    {
        Console.WriteLine(
            // 印出 1. 通訊埠 2. 網址本身 3. 請求方法
            $"通訊埠 : {context.Request.Scheme}, 網址本身 : {context.Request.Headers["Origin"]}, 請求方法 : {context.Request.Method}"
        );
        // 繼續往下走
        await next();
    }
);

app.UseHsts();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
