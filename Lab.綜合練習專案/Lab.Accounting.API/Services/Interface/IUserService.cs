namespace Lab.Accounting.API.Services.Interface;

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

    /// <summary>
    /// 取得使用者資訊
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>使用者資訊</returns>
    Task<ApiResponse<UserResponse>> GetUser(int userId);

    /// <summary>
    /// 取得所有使用者資訊
    /// </summary>
    /// <returns>使用者資訊列表</returns>
    Task<ApiResponse<IEnumerable<UserResponse>>> GetAllUser();

    /// <summary>
    /// 編輯使用者資訊
    /// </summary>
    /// <param name="request">使用者更新資訊</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> UpdateUser(UserUpdateRequest request);

    /// <summary>
    /// 更新使用者密碼
    /// </summary>
    /// <param name="request">舊密碼</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<string>> UpdatePassword(UserUpdatePasswordRequest request);

    /// <summary>
    /// 軟刪除單一用戶
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    Task<ApiResponse<int>> DeleteUser(int userId);
}
