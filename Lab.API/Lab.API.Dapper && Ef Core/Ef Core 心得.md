# Dapper && EFCore 心得

### Dapper 跟 EFCore 主要差別

| 開發方式    | Dapper | Ef Core   |
| :---        |    :----   |         :---- |
| 效率      | 快        | 中   |
| 複雜度   | 較複雜 , 要寫原生 Sql 語法        | 較簡潔      |
| 追蹤   | 無        | 自動追蹤     |


1. 基本 CRUD

```csharp
// 新增( SaveChanges )
[HttpPost("SaveChanges")]
public async Task<IActionResult> InsertBook(BookDTO bookdto)
{
    // 先把 DTO 的投影到 Model
    var book = new Book
    {
        UserId = bookdto.UserId,
        BookName = bookdto.BookName,
        BookPrice = bookdto.BookPrice,
    };
    // 使用 Ef Core Add 新增物件 , 這時候再用 SaveChangesAsync 儲存物件的變更
    context.Add(book);
    await context.SaveChangesAsync();

    return Ok(book);
}
```
```csharp

// 更新( Update )
[HttpPut("Update")]
public async Task<IActionResult> UpdateBook([FromBody] BookDTO bookdto)
{
    // 一般的寫法跟新增差不多 , 改成追蹤 Id 更新原有物件
    var targetbook = await context.Books.FindAsync(bookdto.BookId);
    if (targetbook == null)
        return NotFound();
    targetbook.UserId = bookdto.UserId;
    targetbook.BookName = bookdto.BookName;
    targetbook.BookPrice = bookdto.BookPrice;

    context.Update(targetbook);
    await context.SaveChangesAsync();

    return Ok(targetbook);
}


// 更新( ExecuteUpdate )
[HttpPut("ExecuteUpdate")]
public async Task<IActionResult> ExecuteUpdateBook()
{
    using var tran = await context.Database.BeginTransactionAsync();
    try
    {
        // 改成用 ExecuteUpdate , 這能更快速的批次更新 , 因為它直接跳過記憶體追蹤 , 必須要確定要更新什麼物體
        // 所以用交易包起來 , 失敗就直接返回原點
        var result = await context
            .Books.Where(x => x.UserId == 60012)
            // SetProperty 就能更新指定屬性
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.BookName, x => "阿陳的書"));

        await tran.CommitAsync();
        return Ok(result);
    }
    catch (Exception ex)
    {
        await tran.RollbackAsync();
        throw;
    }
}

```
```csharp
// 查看單筆
[HttpGet("One")]
public async Task<Book> GetBook([FromQuery] int id)
{
    return await context.Books.SingleAsync(x => x.BookId == id);
}

//查看多筆
[HttpGet("All")]
public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks(int page, int pagenumber)
{
    return await context
        .Books.Select(x => new BookDTO
        {
            BookId = x.BookId,
            BookName = x.BookName,
            BookPrice = x.BookPrice,
        })
        // 用Skip跟Take控制顯示數量(分頁)
        .Skip(pagenumber - 1)
        .Take(page)
        .ToListAsync();
}
```
```csharp

// 刪除 ( Remove )
[HttpDelete]
public async Task<bool> DeleteBookAsync([FromQuery] int id)
{
    var book = await context.Books.FindAsync(id);
    if (book != null)
    {
        // Remove 刪除之後 Save 儲存追蹤變更
        context.Books.Remove(book);
        context.SaveChanges();
        return true;
    }
    return false;
}


// 刪除 ( ExecuteDelete )
[HttpDelete("ExecuteDelete")]
public async Task<bool> DeleteBookExecuteDelete()
{
    using var tran = await context.Database.BeginTransactionAsync();
    try
    {
        // 跟 ExecuteUpdate 一樣能高效刪除但會跳過追蹤 , 所以交易包起來
        await context.Books.Where(x => x.UserId == 60012).ExecuteDeleteAsync();
        await tran.CommitAsync();
        return true;
    }
    catch (Exception ex)
    {
        await tran.RollbackAsync();
        throw;
    }
}


```

2. 交易 BeginTransaction

```csharp
// 開啟交易
 using var tran = await context.Database.BeginTransactionAsync();
 try
 {
     var book = new Book
     {
         UserId = bookdto.UserId,
         BookName = bookdto.BookName,
         BookPrice = bookdto.BookPrice,
     };
     context.Add(book);
     await context.SaveChangesAsync();

     // 可以在交易過程設置儲存點 , 這樣當交易失敗時可以不用返回一開始而是返回指定點
     await tran.CreateSavepointAsync("原點!");

     await tran.CommitAsync();
     return Ok(book);
 }
 catch (Exception ex)
 {
     // 錯誤就返回指定點
     await tran.RollbackToSavepointAsync("原點!");
     throw;
 }
```
```csharp
await using var tran = await context.Database.BeginTransactionAsync();
 try
 {
     await context.Books.Where(x => x.UserId == id).ExecuteDeleteAsync();
     // 可以建立多個共用相同連線的內容實例 ( UseTransactionAsync )
     await context.Database.UseTransactionAsync(tran.GetDbTransaction());
     var user = await context.Users.FindAsync(id);
     if (user != null)
     {
         context.Users.Remove(user);
         context.SaveChanges();
         return true;
     }
 }
 catch (Exception ex)
 {
     await tran.RollbackAsync();
     throw;
 }

```

3. Attach , 跟 Update 不同的是他標記完是沒真正更新的狀態 , Update 標記完就更新了

```csharp
  // 更新用戶 ( Attach 測試 )
 [HttpPut("UpdateUser")]
 public async Task UpdateUser(int userId)
 {
     // 只給一個 Pk 值
     var user = new User() { Id = userId };
     
     context.Attach(user);
     // 設定其他屬性
     user.Name = "ANdyyyyy";
     // 這樣就可以騙過 Ef , 跳過查詢步驟了
     await context.SaveChangesAsync();
 }
 

 var targetbook = await context.Books.FindAsync(bookdto.BookId);
 if (targetbook == null)
     return NotFound();
 targetbook.UserId = bookdto.UserId;
 targetbook.BookName = bookdto.BookName;
 targetbook.BookPrice = bookdto.BookPrice;
 // 使用 Attach 標記為 Unchanged , 這時候還沒更新
 context.Attach(targetbook);
 // 變更追蹤狀態為更新
 context.Entry(targetbook).State = EntityState.Modified;
 // 儲存
 await context.SaveChangesAsync();

 return Ok(targetbook);



```




// 1. new 一個實體，只給主鍵：ApplyNo、ID、UserType
var bankTrace = new Reviewer_BankTrace()
{
    ApplyNo = context.ApplyNo,
    ID = mainContext.ID,
    UserType = mainContext.UserType,
};

// 2. Attach → EF 會把它當成「已經在 DB 裡的那一筆」（Unchanged）
_context.Attach(bankTrace);

// 3. 再設其他屬性 → 這些會被標成 Modified
bankTrace.InternalEmailSame_Flag = mainContext.命中檢核行內Email == 命中檢核結果.命中 ? "Y" : "N";
// bankTrace.InternalMobileSame_Flag = ...

// 4. SaveChanges → EF 會生出 UPDATE Reviewer_BankTrace SET ... WHERE ApplyNo = @... AND ID = @... AND UserType = @...
await _context.SaveChangesAsync();