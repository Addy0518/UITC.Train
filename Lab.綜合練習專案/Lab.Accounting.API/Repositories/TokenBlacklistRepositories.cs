using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data;

namespace Lab.Accounting.API.Repositories
{
    public class TokenBlacklistRepositories(DBConnecting connecting) : ITokenBlacklistRepositories
    {
        /// <summary>
        /// 登出後把 token 加入黑名單
        /// </summary>
        /// <param name="jti">Jti 識別碼</param>
        ///  <param name="expirationdate">過期時間</param>
        /// <returns>黑名單Token</returns>
        public async Task<TokenBlackList> AddToken(string jti, DateTime expirationdate)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO tokenblacklist (jti, expirationdate, logoutdate)
                VALUES ( @Jti,
                         @ExpirationDate,
                         @LogoutDate);
                SELECT Cast(Scope_identity() AS INT);";

            return await conn.QuerySingleAsync<TokenBlackList>(
                sql,
                new
                {
                    Jti = jti,
                    ExpirationDate = expirationdate,
                    LogoutDate = DateTime.Now,
                }
            );
        }

        /// <summary>
        /// 比對這個 Token 是否在黑名單
        /// </summary>
        /// <param name="jti">Jti 識別碼</param>
        /// <returns>是否在黑名單</returns>
        public async Task<bool> isBlackList(string jti)
        {
            using var conn = connecting.CreateConnecting();

            var sql = @"Select Top 1 1 From TokenBlackList Where Jti=@jti";

            var result = await conn.ExecuteScalarAsync<int?>(sql, new { jti = jti });

            return result.HasValue;
        }
    }
}
