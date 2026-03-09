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
  public async Task<int> InsertUserMoreTest()
  {
      // 我先加上一個 .Net 內建的功能 StopWatch , 用來計時有無交易的時間差異
      var sw = Stopwatch.StartNew();
      // 使用列舉的範圍 20000 筆加上 Lambda 函式做一個迴圈新增資料
      var users = Enumerable
          .Range(1, 20000)
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
