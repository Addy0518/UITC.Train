using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;

namespace Lab.Accounting.API.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="registerRequest">使用者註冊資訊</param>
        /// <returns>註冊成功</returns>
        Task<ApiResponse<UserResponse>> Register(UserRegisterRequest registerRequest);

        /// <summary>
        /// 使用者登入
        /// </summary>
        /// <param name="loginRequest">使用者登入資訊</param>
        /// <returns>登入成功</returns>
        Task<ApiResponse<UserResponse>> Login(UserLoginRequest loginRequest);

        /// <summary>
        /// 使用者登出
        /// </summary>
        /// <param name="Token">登出的 Token</param>
        /// <returns>是否成功登出</returns>
        Task<ApiResponse<string>> Logout(string Token);

        /// <summary>
        /// 使用者大頭照上傳
        /// </summary>
        /// <param name="userId">使用者 ID </param>
        /// <param name="userFile">使用者大頭照檔案 </param>
        /// <returns>使用者資訊</returns>
        Task<ApiResponse<UserResponse>> UserHeadShotUpload(IFormFile userFile, int userId);
    }
}
