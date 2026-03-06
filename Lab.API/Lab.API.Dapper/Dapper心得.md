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

// 註冊連線
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<TestContext>(options => options.UseSqlServer(connectionString));

// 註冊 UserRepository 的介面服務
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

2. 接下來開始實作 Repository , 第一個是 Insert

```csharp
 public async Task<int> InsertUserAsync(UserInsertDTO userDto)
 {
     // using 連線 , 等結束把資源釋放
     using var conn = _context.Database.GetDbConnection();

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
     using var conn = _context.Database.GetDbConnection();
     {
         var sql = @"Select Name,Email From [User]";

         // 用一般的 Query 查詢就可以 , 我用 DTO 限制只能回傳一些欄位
         var result = await conn.QueryAsync<UserViewDTO>(sql);

         return result.ToList();
     }
 }

 public async Task<UserViewDTO> GetUserAsync(int id)
 {
     using var conn = _context.Database.GetDbConnection();
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
    using var conn = _context.Database.GetDbConnection();
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
    using var conn = _context.Database.GetDbConnection();
    {
        var sql = @"Delete From [User] Where Id=@id";

        return await conn.ExecuteAsync(sql, new { Id = id });
    }
}
```
