using Dapper;
using Lab.API.Dapper.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Lab.API.Dapper.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly TestContext _context;

        public UserRepository(TestContext context)
        {
            _context = context;
        }

        public async Task<int> DeleteUserAsync(int id)
        {
            using var conn = _context.Database.GetDbConnection();
            {
                var sql = @"Delete From [User] Where Id=@id";

                return await conn.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<List<UserViewDTO>> GetAllUsersAsync()
        {
            using var conn = _context.Database.GetDbConnection();
            {
                var sql = @"Select * From [User]";

                // 用一般的 Query 查詢就可以 , 我用 DTO 限制只能回傳一些欄位
                var result = await conn.QueryAsync<UserViewDTO>(sql);

                return result.ToList();
            }
        }

        public async Task<UserViewDTO> GetUserAsync(int id)
        {
            using var conn = _context.Database.GetDbConnection();
            {
                var sql = "Select * From [User] Where Id=@id";

                // new 一個新物件放 Id 進 Sql 裡
                return await conn.QuerySingleAsync<UserViewDTO>(sql, new { Id = id });
            }
        }

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

        public async Task<int> UpdateUserAsync(UserUpdateDTO user)
        {
            using var conn = _context.Database.GetDbConnection();
            {
                var sql =
                    @"Update [User] 
                      Set Name=@Name,Email=@Email,Password=@Password 
                      Where Id=@id";

                // 使用 ExecuteAsync 執行操作 , 他會回傳影響的列數 , 因為 Update 跟 Delete 只要回傳是否有成功 , 所以就回傳列述並判斷 true 或 false
                return await conn.ExecuteAsync(sql, user);
            }
        }
    }
}
