# Template 的學習心得

## 練習了DI注入,CRUD練習,DTO




1.  先在 Program 註冊 AddDbContext , 用來管理資料存取,追蹤實體這些

```csharp

builder.Services.AddDbContext<TodoContext>(options => options.UseInMemoryDatabase("TodoList"));

```

2. 繼承 DbContext 並使用屬性 DbSet , 來使用資料表

```csharp

 public class TodoContext : DbContext
 {
     // 定義跟資料庫溝通的行為
     public TodoContext(DbContextOptions<TodoContext> options)
         : base(options) { }

     // DbSet 則是用來查詢跟儲存 , 把 LINQ 查詢傳成資料庫查詢
     public DbSet<TodoItem> TodoItems { get; set; } = null;
 }

```


3. 創建一個資料表模型,用來儲存資料

```csharp

public class TodoItem

{
    public int Id { get; set; }
    // ? 是允許 null
    public string? Name { get; set; }

    public bool isComplete { get; set; }
    
    public string Password { get; set; }
}

```

4. 實作DTO,控制一些不需要回傳的欄位

```csharp

public class TodoItem
{
    public int Id { get; set; }
    
    public string? Name { get; set; }

    public bool isComplete { get; set; }

    // 本來的 Password 欄位只需要存入不用回傳 , DTO 這裡就可以不寫
}

```



5. DI 注入 && 標記Url名稱

```csharp

[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : Controller
{
 private readonly TodoContext _context;

 // DI 注入 , 在建構時決定要注入什麼,因為是剛開始注入而以所以可以用readonly防止被改動
 public TodoItemsController(TodoContext context)
 {
     _context = context;
 }

 }

```

6. Get 查看所有資料,用分頁方式防止資料一次回傳太多

```csharp

  // Get : api/TodoItems
  [HttpGet]
  // 所有 api 都使用非同步的方式 ( async , await) 防止執行緒卡住
  public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems(int page,int pagenumber)
  {

      return await _context.TodoItems
                   //  toDTO 是我寫的投影 DTO 方法,我放在下面
                   .Select(x =>ToDTO(x))
                   // 用 Skip 跟 Take 控制顯示數量(分頁)
                   .Skip(pagenumber)
                   .Take(page)
                   .ToListAsync();

  }

```

7. ToDTO 投影 DTO ,我把它寫成一個私有方法來共用

```csharp

 // 投影DTO的私有方法
private static TodoItemDTO ToDTO(TodoItem item) =>
new TodoItemDTO
{
    Id = item.Id,
    Name = item.Name,
    isComplete = item.isComplete,
};

```


8. Get 單一資料

```csharp

 // Get : api/TodoItems/id
 [HttpGet("{id}")]
 public async Task<ActionResult<TodoItemDTO>> GetTodoItem(int id)
 {
     // Find 找到指定的資料
     var item = await _context.TodoItems.FindAsync(id);
     if (item == null)
     {
         return NotFound();
     }
     return ToDTO(item);
 }
```


9. Post 新增資料    

```csharp

 // Post : api/TodoItems
[HttpPost]
public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItem item)
{
    // 要存入所有欄位所以不用 DTO
    _context.TodoItems.Add(item);   
    // Add 標記完之後 SaveChangesAsync 非同步儲存所有改變
    await _context.SaveChangesAsync();

    // CreatedAtAction:(string,object,object)=>(1 . URL的名稱 2 . ID  3 . 物件)
    // 創建完成呼叫 get 資料回傳
    return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);

}

```
 
10. Put 更新資料

```csharp

// Put : api/TodoItems/
[HttpPut("{id}")]
public async Task<ActionResult<TodoItemDTO>> PutTodoItem(int id, TodoItemDTO dto)
{
   

    // 確認有沒有這筆資料
    var item = await _context.TodoItems.FindAsync(id);
    // 
    if (!// Put : api/TodoItems/
[HttpPut("{id}")]
public async Task<ActionResult<TodoItemDTO>> PutTodoItem(int id, TodoItemDTO dto)
{
   

    // 確認有沒有這筆資料
    var item = await _context.TodoItems.FindAsync(id);
    // ItemExists 是我寫的檢查重複資料方法
    // 也可以直接在這裡寫 Any , 這裡選擇額外寫一個方法 , 之後也可以加邏輯
    if (!ItemExists(id) || item==null)
    {
        return NotFound();
    }


    item.Name = dto.Name;
    item.isComplete = dto.isComplete;


    try
    {
        await _context.SaveChangesAsync();

    }
    // 抓取所有例外錯誤
    catch (Exception ex)
    {

        throw new Exception($"錯誤 : {ex.Message}");

    }

    return Ok(item);


}(id) || item==null)
    {
        return NotFound();
    }


    item.Name = dto.Name;
    item.isComplete = dto.isComplete;


    try
    {
        await _context.SaveChangesAsync();

    }
    // 抓取所有例外錯誤
    catch (Exception ex)
    {

        throw new Exception($"錯誤 : {ex.Message}");

    }

    return Ok(item);


}

```

11. ItemExists 檢查資料重複


```csharp

// 確認資料是否存在的私有方法
private bool ItemExists(int id)
{
    return _context.TodoItems.Any(x => x.Id == id);
}

```

12. Delete 刪除資料

```csharp

// Delete : api/TodoItems/
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteTodoItem(int id)
{
    var items = await _context.TodoItems.FindAsync(id);
    if (items == null)
    {

        return NotFound();
    }
    // 用Remove刪除記憶體追蹤的物件
    _context.Remove(items);
    // 再儲存
    await _context.SaveChangesAsync();

    // 不用回傳任何東西
    return NoContent();

}

```