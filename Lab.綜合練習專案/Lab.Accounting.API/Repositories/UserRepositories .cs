using Lab.Accounting.API.Common.Responses;

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
        /// 使用者註冊
        /// </summary>
        /// <param name="userInformation">使用者註冊資訊</param>
        /// <returns>使用者資訊</returns>
        public async Task<UserResponse> Login(User userInformation)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select UserId,UserName From [User] Where UserAccount=@UserAccount and UserPassword=@UserPassword";

            return await conn.QuerySingleAsync<UserResponse>(
                sql,
                new
                {
                    UserAccount = userInformation.UserAccount,
                    UserPassword = userInformation.UserPassword,
                }
            );
        }
    }
}
