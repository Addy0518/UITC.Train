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
        Task<User> Login(User userInformation);

        /// <summary>
        /// 檢查使用者是否註冊過
        /// </summary>
        /// <param name="userInformation">使用者註冊資訊</param>
        /// <returns>是否註冊過</returns>
        Task<bool> ExistRegister(User userInformation);

        /// <summary>
        /// 使用者大頭照上傳
        /// </summary>
        /// <param name="userHeadShot">使用者大頭照</param>
        /// <param name="userId">使用者 ID </param>
        /// <returns>影響列數</returns>
        Task<int> UserHeadShotUpload(string userHeadShot, int userId);

        /// <summary>
        /// 取得使用者資訊
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <returns>使用者資訊</returns>
        Task<UserResponse> GetUser(int userId);
    }
}
