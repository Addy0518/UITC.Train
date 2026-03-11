namespace Lab.API.TODO.Services.Interfaces
{
    public interface ITestService
    {
        /// <summary>
        /// 刪除 User
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> DeleteUserAsync(int id);

        /// <summary>
        /// 多筆取得 Users
        /// </summary>
        /// <returns>所有 Users </returns>
        Task<ApiResponse<List<User>>> GetAllUsersAsync();

        /// <summary>
        /// 單筆取得 Users
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>單個 Users </returns>
        Task<ApiResponse<User>> GetUserAsync(int id);

        /// <summary>
        /// 新增 User
        /// </summary>
        /// <param name="inseruser">新增 User 請求</param>
        /// <returns>新增 User</returns>
        Task<ApiResponse<int>> InsertUserAsync(InsertRequest inseruser);

        /// <summary>
        /// 更新 User
        /// </summary>
        /// <param name="updateRequest">更新 User 請求</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> UpdateUserAsync(UpdateRequest updateRequest);
    }
}
