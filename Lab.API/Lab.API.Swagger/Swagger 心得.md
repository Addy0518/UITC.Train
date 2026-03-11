# Swagger 心得


1. 安裝 Swashbuckle.AspNetCore.SwaggerUI 套件來使用 Swagger UI 介面

2. 在 Program 註冊端點 , 在網址輸入 https://localhost:7204\swagger 就有 swagger 介面了 ( 如果要看 openApi 的 Json 打 http://localhost:5000/openapi/v1.json )

```csharp
// Swagger UI
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
});
```

3. Scalar , 一樣安裝套件 Scalar.AspNetCore , 並註冊端點 , 在網址打 https://localhost:7204\scalar\v1 就能用 , 感覺比 swagger 功能齊全

```csharp
// Scaler UI
app.MapScalarApiReference();
```

4. Swagger 在 UI 介面可以註解很多東西 , 讓看的人能第一眼就知道這些 API 在幹嘛

```csharp
 [HttpGet]
 // Summary 概括
 [EndpointSummary("這是Summary")]
 // Description 詳細描述這支 API
 [EndpointDescription("這是Description")]
 // EndpointName 設定端點名稱
 [EndpointName("FromAttributes")]
 // Tags 是給這個 API 分類 , 讓她分到這個 Tag 的區塊
 [Tags("todos", "projects")]
 // Description 給參數掛上標籤
 public IResult Test([Description("This is a description.")] string name)
 {
     return Results.Ok("Hello");
 }

 [HttpPost]
 // 告訴使用者成功的話會是回傳長什麼樣的狀態與資料類型
 [ProducesResponseType<UserDTO>(
     StatusCodes.Status200OK,
     // 回傳格式
     "application/json",
     // 詳細介紹
     Description = "Returns the requested User item."
 )]
 // 這則是失敗 , 沒規定回傳什麼資料型態的話就會照 Task<ActionResult<User>> 裡的模型當預設
 [ProducesResponseType(
     StatusCodes.Status404NotFound,
     Description = "Requested User item not found."
 )]
 // 預設資料類型
 [ProducesDefaultResponseType(Description = "Undocumented status code.")]
 public async Task<ActionResult<User>> InserUser(UserDTO user)
 {
     var user1 = new User
     {
         Name = user.Name,
         Email = user.Email,
         Role = "User",
         Password = user.Password,
     };
     // 使用 Ef Core Add 新增物件 , 這時候再用 SaveChangesAsync 儲存物件的變更
     context.Add(user1);
     await context.SaveChangesAsync();
     return Ok(user1);
 }
```

5.  也可以寫一個 enum 之後加上標籤 , 就不會丟數字而是轉字串回去

```csharp
[JsonConverter(typeof(JsonStringEnumConverter<DayOfTheWeekAsString>))]
public enum DayOfTheWeekAsString
{
    Sunday,
    Monday,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
}

[HttpGet("enum")]
public string GetEnum([FromQuery] DayOfTheWeekAsString num)
{
    // 看外面丟的是否等於列舉裡的 Moday
    bool number = num.HasFlag(DayOfTheWeekAsString.Monday);
    // 回傳是否正確跟在列舉裡的排名
    return $"{number},{(int)num}";
}
```