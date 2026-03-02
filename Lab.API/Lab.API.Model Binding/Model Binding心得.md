# ModelBinding+Model 驗證 + 標籤 的學習心得


1. 在資料進來的時候先進行驗證 , 在 MVC 需要檢查 , 但是 WebAPI 的 [ApiController] 會自帶驗證 (驗證沒過會自動回傳 http 400 ) , 所以不用再寫 if (!ModelState.IsValid)

```csharp

 public async Task<IActionResult> TodoItems(TodoItem item)
 {
     // 資料驗證未過則回傳錯誤訊息
     if (!ModelState.IsValid)
     {
         return BadRequest();
     }

     _todoContext.Add(item);
     await _todoContext.SaveChangesAsync();

     return Ok();
 }
```

2. 接下來是屬性驗證 , 在 Model 或是 DTO 的欄位屬性加上標籤來驗證 , 這裡就可以在資料進來時做防護


```csharp

[Required] // 必填
[StringLength(100)] // 字串長度
public string? Name { get; set; } = null; // 預設值是 null

[DataType(DataType.Date)] // 欄位型態
[Display(Name = "開始日期")] // 欄位名稱
public DateTime StartDate { get; set; }

[Range(0, 9999)] // 值的範圍
public decimal Price { get; set; }

```

3. 或者也可以自訂標籤規則

```csharp

public class TodoItem : IValidatableObject // 先實作模型驗證介面



 public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
 {
     // 先拿到 Db 的服務
     TodoContext _todoContext = (TodoContext)
         validationContext.GetService(typeof(TodoContext));

     bool isDistinct = _todoContext.TodoItems.Any(x => x.Name == Name);

     // 先確認有沒有資料
     if (_todoContext != null)
     {
        // 開始自訂規則
         if (isDistinct)
         {
             // 因為回傳錯誤訊息是 IEnumerable 會有多個 , 所以用 yield 來疊加訊息再一次回傳
             yield return new ValidationResult("名稱重複!");
         }

         if (StartDate > EndDate)
         {
             yield return new ValidationResult("開始日期不能大於結束日期!");
         }
     }
 }

```

4. 在標籤裡也可以設定要回傳什麼錯誤訊息

```csharp

[StringLength(10, ErrorMessage = "超過長度了!")] 

```

5. 要允許 null 的話就加上 ? ,預設的跟 Required 差別是一個回傳預設錯誤訊息 , Required則可以自訂

```csharp

public string? Name { get; set; }

```

6. 設定參數的來源

   1. [FromQuery] -取得查詢字串中的值。
   2. [FromRoute] -從路由資料取得值。
   3. [FromForm] -從張貼的表單欄位取得值。
   4. [FromBody] -從要求主體取得值。
   5. [FromHeader] -取得 HTTP 標頭的值。

```csharp

  [HttpGet("{id}")]
  public async Task<IActionResult> GetTodoItem([FromRoute] int id) // 從路由得值查詢

```

