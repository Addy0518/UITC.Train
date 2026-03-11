using Lab.API.TODO.Infrastructures.Data.Entites;
using Lab.API.TODO.Repositories.Interfaces;

namespace Lab.API.TODO.Repositories.Implements
{
    public class TestRepository(TestConnection connection) : ITestRepository
    {
        /// <summary>
        /// 刪除 User
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>影響列數</returns>
        public async Task<int> DeleteUserAsync(int id)
        {
            using var conn = connection.CreateConnection();
            {
                var sql = @"Delete From [User] Where Id=@id";

                return await conn.ExecuteAsync(sql, new { Id = id });
            }
        }

        /// <summary>
        /// 多筆取得 Users
        /// </summary>
        /// <returns>所有 Users </returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            using var conn = connection.CreateConnection();
            {
                var sql = @"Select * From [User]";

                // 用一般的 Query 查詢就可以 , 我用 DTO 限制只能回傳一些欄位
                var result = await conn.QueryAsync<User>(sql);

                return result.ToList();
            }
        }

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

        /// <summary>
        /// 新增 User
        /// </summary>
        /// <param name="Name">名稱</param>
        /// <param name="Role">名稱</param>
        /// <param name="Email">名稱</param>
        /// <param name="Password">名稱</param>
        /// <returns>新增 User</returns>
        public async Task<int> InsertUserAsync(
            string Name,
            string Role,
            string Email,
            string Password
        )
        {
            // using 連線 , 等結束把資源釋放
            using var conn = connection.CreateConnection();

            // 建立 Sql 語法
            var sql =
                "Insert Into [User] (Name,Role,Email,Password) Values (@Name,@Role,@Email,@Password) Select Cast(Scope_Identity() as int);";

            // 投影新增的資料
            var result = new User
            {
                Name = Name,
                Role = Role is not null ? Role : "User",
                Email = Email,
                Password = Password,
            };
            // QuerySingleAsync 查詢唯一符合條件的資料 , 再丟 Pk 回去
            return await conn.QuerySingleAsync<int>(sql, result);
        }

        /// <summary>
        /// 更新 User
        /// </summary>
        /// <param name="Id">名稱</param>
        /// <param name="Name">名稱</param>
        /// <param name="Email">名稱</param>
        /// <returns>影響列數</returns>
        public async Task<int> UpdateUserAsync(int Id, string Name, string Email)
        {
            using var conn = connection.CreateConnection();
            {
                var sql =
                    @"Update [User] 
                      Set Name=@Name,Email=@Email 
                      Where Id=@Id";

                // 使用 ExecuteAsync 執行操作 , 他會回傳影響的列數 , 因為 Update 跟 Delete 只要回傳是否有成功 , 所以就回傳列述並判斷 true 或 false
                return await conn.ExecuteAsync(
                    sql,
                    new
                    {
                        Id = Id,
                        Name = Name,
                        Email = Email,
                    }
                );
            }
        }
    }
}
