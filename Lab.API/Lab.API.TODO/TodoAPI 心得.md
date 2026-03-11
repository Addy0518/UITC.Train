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

 // 讀取 XML 檔案設定
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
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

3. 開始做三層式 , 我這邊只放一個不然太多

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
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<User>))]
public async Task<IActionResult> GetUserAsync(int id)
{
    return Ok(await testservice.GetUserAsync(id));
}
```

### 目前做到這邊明天繼續