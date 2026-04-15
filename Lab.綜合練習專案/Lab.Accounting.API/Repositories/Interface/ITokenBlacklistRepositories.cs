namespace Lab.Accounting.API.Repositories.Interface
{
    public interface ITokenBlacklistRepositories
    {
        /// <summary>
        /// 登出後把 token 加入黑名單
        /// </summary>
        /// <param name="jti">Jti 識別碼</param>
        ///  <param name="expirationdate">過期時間</param>
        /// <returns>黑名單Token</returns>
        Task<TokenBlackList> AddToken(string jti, DateTime expirationdate);

        /// <summary>
        /// 比對這個 Token 是否在黑名單
        /// </summary>
        /// <param name="jti">Jti 識別碼</param>
        /// <returns>是否是黑名單</returns>
        Task<bool> isBlackList(string jti);
    }
}
