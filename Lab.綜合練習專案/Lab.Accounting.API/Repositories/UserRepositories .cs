using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class UserRepositories(DBConnecting connecting) : IUserRepositories
    {
        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="userInformation">使用者註冊資訊</param>
        /// <returns>使用者資訊</returns>
        public async Task<UserResponse> Register(User userInformation)
        {
            using var conn = connecting.CreateConnecting();
            var sql =
                @"Insert Into [User] (
                  UserName, UserAccount, UserPassword, UserPhone
                ) 
                values 
                  (
                    @UserName, @UserAccount, @UserPassword, 
                    @UserPhone
                  );
                Select 
                  Cast(
                    Scope_Identity() as int
                  );";

            return await conn.QuerySingleAsync<UserResponse>(sql, userInformation);
        }

        /// <summary>
        /// 檢查使用者是否註冊過
        /// </summary>
        /// <param name="userInformation">使用者註冊資訊</param>
        /// <returns>是否註冊過</returns>
        public async Task<bool> ExistRegister(User userInformation)
        {
            using var conn = connecting.CreateConnecting();
            var sql = @"Select Top 1 1 From [User] Where @UserAccount=UserAccount";

            var result = await conn.ExecuteScalarAsync<int?>(sql, userInformation);
            // 有值就存在 , 沒值就不存在
            return result.HasValue;
        }

        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="userInformation">使用者註冊資訊</param>
        /// <returns>使用者資訊</returns>
        public async Task<User> Login(User userInformation)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select UserId,UserName,UserRole,UserPassword,UserAddress From [User] Where UserAccount=@UserAccount";

            return await conn.QueryFirstOrDefaultAsync<User>(sql, new { UserAccount = userInformation.UserAccount });
        }

        /// <summary>
        /// 使用者大頭照上傳
        /// </summary>
        /// <param name="userHeadShot">使用者大頭照</param>
        /// <param name="userId">使用者 ID </param>
        /// <returns>影響列數</returns>
        public async Task<int> UserHeadShotUpload(string userHeadShot, int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Update 
                  [User] 
                Set 
                  UserHeadShot = COALESCE(@UserHeadShot, UserHeadShot)
                where 
                  UserId = @UserId";

            return await conn.ExecuteAsync(sql, new { UserHeadShot = userHeadShot, UserId = userId });
        }

        /// <summary>
        /// 取得使用者資訊
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <returns>使用者資訊</returns>
        public async Task<UserResponse> GetUser(int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select UserId,UserName,UserHeadShot From [User]
                where 
                  UserId = @UserId";

            return await conn.QueryFirstOrDefaultAsync<UserResponse>(sql, new { UserId = userId });
        }
    }
}
