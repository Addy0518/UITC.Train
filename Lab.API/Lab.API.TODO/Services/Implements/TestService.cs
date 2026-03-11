namespace Lab.API.TODO.Services.Implements
{
    public class TestService(ITestRepository repository) : ITestService
    {
        /// <summary>
        /// 刪除 User
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> DeleteUserAsync(int id)
        {
            var user = await repository.GetUserAsync(id);
            if (user == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            await repository.DeleteUserAsync(id);

            return ApiResponseHelper.Success(id, "成功");
        }

        /// <summary>
        /// 多筆取得 Users
        /// </summary>
        /// <returns>所有 Users </returns>
        public async Task<ApiResponse<List<User>>> GetAllUsersAsync()
        {
            var data = await repository.GetAllUsersAsync();
            return ApiResponseHelper.Success(data, "成功");
        }

        /// <summary>
        /// 單筆取得 Users
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>單個 Users </returns>
        public async Task<ApiResponse<User>> GetUserAsync(int id)
        {
            var user = await repository.GetUserAsync(id);
            if (user == null)
            {
                return ApiResponseHelper.NotFound<User>();
            }

            return ApiResponseHelper.Success(user, "成功");
        }

        /// <summary>
        /// 新增 User
        /// </summary>
        /// <param name="inseruser">新增 User 請求</param>
        /// <returns>新增 User</returns>
        public async Task<ApiResponse<int>> InsertUserAsync(InsertRequest inseruser)
        {
            var user = await repository.InsertUserAsync(
                inseruser.Name,
                inseruser.Role,
                inseruser.Email,
                inseruser.Password
            );
            return ApiResponseHelper.Success(user, "成功");
        }

        /// <summary>
        /// 更新 User
        /// </summary>
        /// <param name="updateRequest">更新 User 請求</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UpdateUserAsync(UpdateRequest updateRequest)
        {
            var user = await repository.GetUserAsync(updateRequest.Id);
            if (user == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            await repository.UpdateUserAsync(
                updateRequest.Id,
                updateRequest.Name,
                updateRequest.Email
            );
            return ApiResponseHelper.Success(updateRequest.Id, "成功");
        }
    }
}
