using System.Data;
using System.Diagnostics;
using Dapper;
using Lab.API.Dapper.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lab.API.Dapper.Repository
{
    public class UserRepository(UserConnection connection, TestContext context) : IUserRepository
    {
        public async Task<int> DeleteUserAsync(int id)
        {
            using var conn = connection.CreateConnection();
            {
                var sql = @"Delete From [User] Where Id=@id";

                return await conn.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<List<UserViewDTO>> GetAllUsersAsync()
        {
            using var conn = connection.CreateConnection();
            {
                var sql = @"Select * From [User]";

                // 用一般的 Query 查詢就可以 , 我用 DTO 限制只能回傳一些欄位
                var result = await conn.QueryAsync<UserViewDTO>(sql);

                return result.ToList();
            }
        }

        public async Task<UserViewDTO> GetUserAsync(int id)
        {
            using var conn = connection.CreateConnection();
            {
                var sql = "Select * From [User] Where Id=@id";

                // new 一個新物件放 Id 進 Sql 裡
                return await conn.QuerySingleAsync<UserViewDTO>(sql, new { Id = id });
            }
        }

        public async Task<int> InsertUserAsync(UserInsertDTO userDto)
        {
            // using 連線 , 等結束把資源釋放
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

        public async Task<int> UpdateUserAsync(UserUpdateDTO user)
        {
            using var conn = connection.CreateConnection();
            {
                var sql =
                    @"Update [User] 
                      Set Name=@Name,Email=@Email,Password=@Password 
                      Where Id=@id";

                // 使用 ExecuteAsync 執行操作 , 他會回傳影響的列數 , 因為 Update 跟 Delete 只要回傳是否有成功 , 所以就回傳列述並判斷 true 或 false
                return await conn.ExecuteAsync(sql, user);
            }
        }

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

        public async Task<UserAndBooksDTO> GetBooksAndUsermerge(int id)
        {
            // 要確保 Dapper 跟 Ef Core 都在同一個連線交易 , 所以 Dapper 連線也改從 Db 拿
            var conn = context.Database.GetDbConnection();

            using (var trn = context.Database.BeginTransaction())
            {
                var book = await context.Books.Where(x => x.UserId == id).ToListAsync();

                var sql = "Select * From [User] Where Id=@id";

                var userdto = await conn.QuerySingleAsync<User>(
                    sql,
                    new { Id = id },
                    // 加入 Ef 的交易
                    transaction: trn.GetDbTransaction()
                );
                // 加入 DTO
                var result = new UserAndBooksDTO { books = book, users = userdto };

                return result;
            }
        }

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

            //using var userconn = connection.CreateConnection();
            //string userSql =
            //         "Update [User] Set Name=@Name,Email=@Email,Password=@Password  Where Id=@Id";
            //var userQuery = userconn.ExecuteAsync(userSql, dto);

            //using var bookconn = connection.CreateConnection();
            //string bookSql =
            //        "Update [Books] Set BookName=@BookName,BookPrice=@BookPrice Where UserId=@UserId";
            //var bookQuery = bookconn.ExecuteAsync(bookSql, dto.UserBooks);
            //await Task.WhenAll(userQuery, bookQuery);

            // WhenAll 則會等待這兩個執行緒都完成時 , 才會繼續執行
            await Task.WhenAll(userTask, bookTask);

            return true;
        }

        public async Task<int> InsertUserChunkTest()
        {
            // 我先加上一個 .Net 內建的功能 StopWatch , 用來計時有無交易的時間差異
            var sw = Stopwatch.StartNew();
            // 紀錄最後回傳的總影響行數
            int totalcount = 0;

            // 使用列舉的範圍 20000 筆加上 Lambda 函式做一個迴圈新增資料
            var allusers = Enumerable
                .Range(1, 20000)
                .OrderBy(x => x)
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

            // 使用 Chunk 分割總量變成 5000 每筆
            var chunks5000 = allusers.Chunk(5000);

            foreach (var chunk in chunks5000)
            {
                // 因為是用迴圈批次儲存 , 所以交易移到迴圈裡 , 每存一次開啟一次交易 , 確保當某一批失敗時前面的不會失敗
                using var tran = await conn.BeginTransactionAsync();
                try
                {
                    // Dapper 會自動展開存入資料 , 所以這裡一樣正常加入參數就好
                    var sql =
                        "Insert Into [User] (Name,Role,Email,Password) Values (@Name,@Role,@Email,@Password);";
                    var result = await conn.ExecuteAsync(sql, chunk, tran);
                    totalcount += result;
                    tran.CommitAsync();
                }
                catch (Exception ex)
                {
                    tran.RollbackAsync();
                    return 0;
                    throw;
                }
            }
            // 成功後就停止 StopWatch , 看計算的時間
            sw.Stop();
            Console.WriteLine($"耗時 {sw.ElapsedMilliseconds} ms");
            return totalcount;
        }

        public async Task<int> InsertUserTest()
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
    }
}
