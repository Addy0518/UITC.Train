# Exception Handing 心得 

### 處理程式碼發生的錯誤異常 , 處理能想到的所有錯誤

1. 基礎的處理錯誤與法

```csharp
[HttpPost]
public async Task<IActionResult> TestCreate(Test test)
{
    // try : 嘗試執行執行區塊內的程式碼 , 測試是否有錯誤
    try
    {
        _context.Add(test);
        await _context.SaveChangesAsync();

        return Ok();
    }
    // catch : 捕捉一切拋出的錯誤
    // FormatException : 格式錯誤
    catch (FormatException fex)
    {
        // throw : 扔出異常
        throw new Exception("格式錯誤!");
    }
    // Exception : 通常放在最後 , 抓一切沒抓到的錯誤
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
    // 不管有無錯誤 , 最後都會執行
    finally { }
}
```

2. Exception Filter , 抓取 Action 及 Action Filter 所發出的 Exception , 但其他像是 Middleware 的錯誤就抓不到 , 所以要做全域的捕捉器比較沒那麼合適 , 但還是來試試看

```csharp
 // 建立 ExceptionFilter
 public class ExceptionFilter : IAsyncExceptionFilter
 {
     public Task OnExceptionAsync(ExceptionContext context)
     {
         // 非同步拿到錯誤訊息
         context.HttpContext.Response.WriteAsync(
             $"{GetType().Name} catch exception. Message: {context.Exception.Message}"
         );
         // 回傳已完成的工作
         return Task.CompletedTask;
     }
 }


 // 註冊 ExceptionFilter
builder.Services.AddControllers(
    (options) =>
    {
        options.Filters.Add<ExceptionFilter>();
    }
);
```

3. Exception Middleware , 註冊在所有 Middleware 的最外層，就可以變成全域的 Exception Handler

```csharp
 public class ExceptionMiddleware
 {
     private readonly RequestDelegate _next;

     public ExceptionMiddleware(RequestDelegate next)
     {
         _next = next;
     }

     public async Task Invoke(HttpContext context)
     {
         try
         {
             // 當Middleware往下一步走時 ,拿到委派傳輸的訊息 , 有錯誤就 catch
             await _next(context);
         }
         catch (Exception ex)
         {
             await context.Response.WriteAsync(
                 $"{GetType().Name} catch Exception : {ex.Message}"
             );
         }
     }
 }

 // 註冊自訂的 Middleware
app.UseMiddleware<ExceptionMiddleware>();


// 抓到錯誤的範例
ExceptionFilter catch exception. Message: An error occurred while saving the entity changes. See the inner exception for details.ExceptionMiddleware 抓到錯誤 : An error occurred while saving the entity changes. See the inner exception for details.
``` 

4. UseExceptionHandler , 讓所有異常都在這裡處理 , 不過就不能處理太複雜的邏輯 

```csharp
// UseExceptionHandler 任何未處理的異常都會在這裡被抓到
app.UseExceptionHandler(options =>
{
    // options.Run 處裡請求 , 但不呼叫下一個物件
    options.Run(async context =>
    {
        // Http 狀態設為 500 , 回覆內容設定為Json
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        // IExceptionHandlerFeature 允許存取原始異常 , 異常的詳細資訊會存入這個功能中
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature is not null)
        {
            // 回復異常訊息
            var error = new { message = "An unexpected error occurred" };
            await context.Response.WriteAsJsonAsync(error);
        }
    });
});

```

5. IExceptionHandler 自訂異常

```csharp
 // abstract 用抽象類別讓別人不能直接使用 AppException 通用錯誤訊息 , 只能自己在開發比如 NotFoundException
 public abstract class AppException : Exception
 {
     // 用 HttpStatusCode 決定要回傳什麼狀態碼
     public HttpStatusCode StatusCode { get; }

     // 用建構函式接一個訊息 Message 跟狀態碼
     protected AppException(
         string Message,
         HttpStatusCode statusCode = HttpStatusCode.InternalServerError
     )
         : base(Message)
     {
         StatusCode = statusCode;
     }
 }

 // 用 sealed 讓函式不被繼承
 public sealed class NotFoundException : AppException
 {
     public NotFoundException(string resourceName, object key)
         : base(
             $"{resourceName} with identifier '{key}' was not found.",
             HttpStatusCode.NotFound
         ) { }
 }

 public sealed class BadRequestException : AppException
 {
     public BadRequestException(string message)
         : base(message, HttpStatusCode.BadRequest) { }
 }

 public sealed class ConflictException : AppException
 {
     public ConflictException(string message)
         : base(message, HttpStatusCode.Conflict) { }
 }

 public sealed class ValidationException : AppException
 {
     public IDictionary<string, string[]> Errors { get; }

     public ValidationException(IDictionary<string, string[]> errors)
         : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
     {
         Errors = errors;
     }

     public ValidationException(string field, string error)
         : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
     {
         Errors = new Dictionary<string, string[]> { { field, [error] } };
     }
 }


 // 實際使用
  [HttpGet("{id}")]
 public async Task<IActionResult> Get(int id)
 {
     var result = await _context.Tests.FindAsync(id);
     if (result == null)
     {
         throw new NotFoundException("Product", id);
     }
     return Ok(result);
 }
```