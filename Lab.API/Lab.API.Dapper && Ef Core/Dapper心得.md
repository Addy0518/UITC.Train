# Dapper && EFCore 心得

### Dapper 跟 EFCore 我會合併成一個專案 , 並分成兩張表去做 CRUD  

1. 首先是前置工作 , 我製作了兩張表 , 一張用 Dapper 一張用 EFCore , 再來註冊 Repository 跟 注入連線

| 資料表     | User | Books   |
| :---        |    :----   |         :---- |
| 1      | Id       | BookId   |
| 2   | Name        | UserId      |
| 3   | Role        | BookName     |
| 4   | Email        | BookPrice      |
| 5   | Password       |     |


```csharp

// 使用 Dapper 方式的注入連線 , 這樣寫是注入的簡寫
public class UserConnection(IConfiguration configuration)
{
    // 新增一個連線方法拿到 Setting 連線
    public SqlConnection CreateConnection()
    {
        return new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    }
}


// 分離出來一個註冊連線跟 Service 的類別 , 在註冊到 Program
public static class AddDIConfig
{
    // 注入服務
    public static void ADDDIConfig(this IServiceCollection services)
    {
        // 使用剛剛設定的 User 連線
        services.AddSingleton<UserConnection>();
        // 註冊 Repo 介面
        services.AddScoped<IUserRepository, UserRepository>();
    }
}


// 最後在 Program 註冊
builder.Services.ADDDIConfig();
```

2. 接下來開始實作 Repository , 第一個是 Insert

```csharp
 public async Task<int> InsertUserAsync(UserInsertDTO userDto)
 {
     // using 連線 , 等結束把資源釋放 , 這裡沒有使用大括號 , 所以最後連線會自動在外一層大括號釋放
     using var conn = connection.CreateConnection();

     // 建立 Sql 語法
     var sql =
         "Insert Into [User] (Name,Role,Email,Password) Values (@Name,@Role,@Email,@Password) Select Cast(Scope_Identity() as int);";

     // 投影新增的資料
     var result = new User
     {
         Name = userDto.Name,
         Role = "User",
         Email = userDto.Email,
         Password = userDto.Password,
     };
     // QuerySingleAsync 查詢唯一符合條件的資料 , 再丟 Pk 回去
     return await conn.QuerySingleAsync<int>(sql, result);
 }

```

2. 第二個是 Get 跟 GetAll

```csharp
 public async Task<List<UserViewDTO>> GetAllUsersAsync()
 {
    // 這裡有使用大括號 , 所以連線會在這層括號釋放
     using var conn = connection.CreateConnection();
     {
         var sql = @"Select Name,Email From [User]";

         // 用一般的 Query 查詢就可以 , 我用 DTO 限制只能回傳一些欄位
         var result = await conn.QueryAsync<UserViewDTO>(sql);

         return result.ToList();
     }
 }

 public async Task<UserViewDTO> GetUserAsync(int id)
 {
     using var conn = connection.CreateConnection();
     {
         var sql = "Select Name,Email From [User] Where Id=@id";

         // new 一個新物件放 Id 進 Sql 裡 , 用 QuerySingle 查找唯一值
         return await conn.QuerySingleAsync<UserViewDTO>(sql, new { Id = id });
     }
 }
```

3. Update 跟 Delete

```csharp
// Update
public async Task<int> UpdateUserAsync(UserUpdateDTO user)
{
    using var conn = connection.CreateConnection();
    {
        var sql =
            @"Update [User] 
              Set Name=@Name,Email=@Email,Password=@Password 
              Where Id=@id";
        // 使用 ExecuteAsync 執行操作 , 他會回傳影響的列數 
        // 因為 Update 跟 Delete 只要回傳是否有成功 , 所以就回傳列述並判斷 true 或 false
        return await conn.ExecuteAsync(sql, user);
    }
}

// Delete
public async Task<int> DeleteUserAsync(int id)
{
    using var conn = connection.CreateConnection();

    var sql = @"Delete From [User] Where Id=@id";

    return await conn.ExecuteAsync(sql, new { Id = id });
    
}
```

3. Dapper  在一次連線中抓取多個結果 => QueryMultiple

```csharp
 public async Task<UserAndBooksDTO> GetBooksAndUser(int id)
 {
     // 建立兩個 SQL 查詢兩張表
     var sql = "Select * From [User] Where Id=@id ;Select * From [Books] Where UserID=@id";

     using var conn = connection.CreateConnection();
     {
         // 使用非同步 QueryMultiple 做多個 SQL 查詢 , 放入 Sql 跟 Id 參數
         using (var multi = await conn.QueryMultipleAsync(sql, new { Id = id }))
         {
             // 使用 Read 把結果轉物件
             var user = multi.Read<User>().FirstOrDefault();
             var book = multi.Read<Book>().ToList();
             return new UserAndBooksDTO { users = user, books = book };
         }
     }
 }
```

4. Dapper 的併發查詢 => WhenAll

```csharp
public async Task<bool> UpdateUserAndBooks(UserUpdateDTO dto)
{
    // 建立兩個執行緒來同時進行兩個連線
    var userTask = Task.Run(async () =>
    {
        using var userconn = connection.CreateConnection();
        {
            string userSql =
                "Update [User] Set Name=@Name,Email=@Email,Password=@Password  Where Id=@Id";
            var userQuery = await userconn.ExecuteAsync(userSql, dto);
        }
    });

    var bookTask = Task.Run(async () =>
    {
        using var bookconn = connection.CreateConnection();
        {
            string bookSql =
                "Update [Books] Set BookName=@BookName,BookPrice=@BookPrice Where UserId=@UserId";
            var bookQuery = await bookconn.ExecuteAsync(bookSql, dto.UserBooks);
        }
    });
    // WhenAll 則會等待這兩個執行緒都完成時 , 才會繼續執行
    await Task.WhenAll(userTask, bookTask);

    return true;
}
```

5. 批次新增時得隱式轉換以及 Transation 的同時 Commit => 最後計算結果 : 有用交易是 5 秒 , 沒用是 6.6 秒

```csharp
 public async Task<int> InsertUserTest()
 {
     // 我先加上一個 .Net 內建的功能 StopWatch , 用來計時有無交易的時間差異
     var sw = Stopwatch.StartNew();
     // 使用列舉的範圍 20000 筆加上 Lambda 函式做一個迴圈新增資料
     var users = Enumerable
         .Range(1, 100000)
         .Select(i =>
         {
             // 新增 Parameters 物件來防止隱式轉型
             var param = new DynamicParameters();
             param.Add("Name", $"User{i}", System.Data.DbType.String, size: 50);
             param.Add("Role", $"User", System.Data.DbType.String, size: 50);
             param.Add("Email", $"Email{i}", System.Data.DbType.String, size: 100);
             param.Add("Password", $"Password{i}", System.Data.DbType.String, size: 50);

             return param;
         })
         .ToList();

     using var conn = connection.CreateConnection();
     // 非同步開啟交易
     await conn.OpenAsync();

     using var tran = conn.BeginTransaction();

     try
     {
         // Dapper 會自動展開存入資料 , 所以這裡一樣正常加入參數就好
         var sql =
             "Insert Into [User] (Name,Role,Email,Password) Values (@Name,@Role,@Email,@Password);";
         var result = await conn.ExecuteAsync(sql, users, tran);

         if (result == users.Count)
         {
             // 成功後就停止 StopWatch , 看計算的時間
             sw.Stop();
             Console.WriteLine($"耗時 {sw.ElapsedMilliseconds} ms");
             tran.Commit();
             // 回傳成功新增了幾筆
             return result;
         }
         else
         {
             tran.Rollback();
             return 0;
         }
     }
     catch (Exception ex)
     {
         throw;
     }
 }
```

6. 新增一個分批儲存的版本 , 本來是一次 Commit , 換每五千 Commit 一次 => 沒分批耗時 : 4337 ms , 有分批耗時 : 4607 ms , 目前有成功分批 , 但是我發現 Dapper 在寫入時好像不會做分批的動作 , 所以耗時沒什麼差別 , 甚至因為每一次迴圈都開一次交易所以耗時更久一點 , 我看 Dapper 要實際做到分批存入要其他特殊的語法?

```csharp
 public async Task<int> InsertUserChunkTest()
 {

     var sw = Stopwatch.StartNew();
     // 紀錄最後回傳的總影響行數
     int totalcount = 0;

     var allusers = Enumerable
         .Range(1, 20000)
         .OrderBy(x => x)
         .Select(i =>
         {

             var param = new DynamicParameters();
             param.Add("Name", $"User{i}", System.Data.DbType.String, size: 50);
             param.Add("Role", $"User", System.Data.DbType.String, size: 50);
             param.Add("Email", $"Email{i}", System.Data.DbType.String, size: 100);
             param.Add("Password", $"Password{i}", System.Data.DbType.String, size: 50);

             return param;
         })
         .ToList();

     using var conn = connection.CreateConnection();

     await conn.OpenAsync();

     // 使用 Chunk 分割總量變成 5000 每筆
     var chunks5000 = allusers.Chunk(5000);

     foreach (var chunk in chunks5000)
     {
         // 因為是用迴圈批次儲存 , 所以交易移到迴圈裡 , 每存一次開啟一次交易 , 確保當某一批失敗時前面的不會失敗
         using var tran = conn.BeginTransaction();
         try
         {
     
             var sql =
                 "Insert Into [User] (Name,Role,Email,Password) Values (@Name,@Role,@Email,@Password);";
             var result = await conn.ExecuteAsync(sql, chunk, tran);
             totalcount += result;
             tran.Commit();
         }
         catch (Exception ex)
         {
             tran.Rollback();
             return 0;
             throw;
         }
     }
  
     sw.Stop();
     Console.WriteLine($"耗時 {sw.ElapsedMilliseconds} ms");
     return totalcount;
 }
```

7. SqlBulkCopy , 能解決上述兩個儲存慢的問題 => 耗時 : 0 ms

```csharp
public void InserUserSqlBulkTest()
{
    var sw = new Stopwatch();

    // 建立 datatable 對應資料庫欄位
    DataTable dt = new DataTable();
    dt.Columns.Add("Name", typeof(string));
    dt.Columns.Add("Role", typeof(string));
    dt.Columns.Add("Email", typeof(string));
    dt.Columns.Add("Password", typeof(string));

    for (int i = 0; i < 10000; i++)
    {
        // 迴圈把每一列測試資料加入
        DataRow dr = dt.NewRow();
        dr["Name"] = $"Name{i}";
        dr["Role"] = "User";
        dr["Email"] = "aaa";
        dr["Password"] = "bbb";

        dt.Rows.Add(dr);
    }

    using var conn = connection.CreateConnection();
    conn.Open();

    // 使用 SqlBulkCopy 內建套件 , 加入連線
    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
    {
        // 對應資料庫名稱
        bulkCopy.DestinationTableName = "[dbo].[User]";
        // 寫入資料庫
        bulkCopy.WriteToServer(dt);
        // 成功後就停止 StopWatch , 看計算的時間
        sw.Stop();
        Console.WriteLine($"耗時 {sw.ElapsedMilliseconds} ms");
    }
}
```



