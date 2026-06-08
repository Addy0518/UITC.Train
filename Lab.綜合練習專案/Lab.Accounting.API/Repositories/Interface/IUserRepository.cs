using Lab.Accounting.API.Common.Requests.Category;

namespace Lab.Accounting.API.Repositories.Interface;

public interface IUserRepository
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

    /// <summary>
    /// 取得使用者詳細資訊 ( 管理員 )
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>使用者詳細資訊</returns>
    Task<UserResponse> GetUserDetails(int userId);

    /// <summary>
    /// 取得所有使用者資訊
    /// </summary>
    ///  <param name="request">搜尋使用者請求 </param>
    /// <returns>使用者資訊列表</returns>
    Task<IEnumerable<UserResponse>> GetAllUser(UserSearchRequest request);

    /// <summary>
    /// 編輯使用者資訊
    /// </summary>
    /// <param name="request">使用者更新資訊</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateUser(UserUpdateRequest request);

    /// <summary>
    /// 查看使用者密碼
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>影響列數</returns>
    Task<User> GetUserPassword(int userId);

    /// <summary>
    /// 更新使用者密碼
    /// </summary>
    /// <param name="request">舊密碼</param>
    /// <returns>影響列數</returns>
    Task<int> UpdatePassword(UserUpdatePasswordRequest request);

    /// <summary>
    /// 改變權限變賣家
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <param name="userRole">使用者權限 </param>
    /// <returns>影響列數</returns>
    Task<int> UpdateRole(int userId, string userRole);

    /// <summary>
    /// 軟刪除單一用戶
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <param name="adminId">管理員 ID</param>
    /// <param name="deleteReason">停用原因</param>
    /// <returns>影響列數</returns>
    Task<int> DeleteUser(int userId, int adminId, string deleteReason);

    /// <summary>
    /// 復原已選取的用戶刪除狀態
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateUserDeleteStatus(int userId);
}
