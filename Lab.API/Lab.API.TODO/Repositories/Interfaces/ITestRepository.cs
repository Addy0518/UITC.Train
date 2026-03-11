namespace Lab.API.TODO.Repositories.Interfaces
{
    public interface ITestRepository
    {
        /// <summary>
        /// 刪除 User
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>影響列數</returns>
        Task<int> DeleteUserAsync(int id);

        /// <summary>
        /// 多筆取得 Users
        /// </summary>
        /// <returns>所有 Users </returns>
        Task<List<User>> GetAllUsersAsync();

        /// <summary>
        /// 單筆取得 Users
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>單個 Users </returns>
        Task<User> GetUserAsync(int id);

        /// <summary>
        /// 新增 User
        /// </summary>
        /// <param name="Name">名稱</param>
        /// <param name="Role">名稱</param>
        /// <param name="Email">名稱</param>
        /// <param name="Password">名稱</param>
        /// <returns>新增 User</returns>
        Task<int> InsertUserAsync(string Name, string Role, string Email, string Password);

        /// <summary>
        /// 更新 User
        /// </summary>
        /// <param name="Id">名稱</param>
        /// <param name="Name">名稱</param>
        /// <param name="Email">名稱</param>
        /// <returns>影響列數</returns>
        Task<int> UpdateUserAsync(int Id, string Name, string Email);
    }
}
