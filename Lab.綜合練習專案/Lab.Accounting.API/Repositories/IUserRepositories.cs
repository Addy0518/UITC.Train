using Lab.Accounting.API.Common.Responses;

namespace Lab.Accounting.API.Repositories
{
    public interface IUserRepositories
    {
        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="userInformation">使用者註冊資訊</param>
        /// <returns>使用者資訊</returns>
        Task<UserResponse> Register(User userInformation);

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="userInformation">使用者登入資訊</param>
        /// <returns>使用者資訊</returns>
        Task<UserResponse> Login(User userInformation);
    }
}
