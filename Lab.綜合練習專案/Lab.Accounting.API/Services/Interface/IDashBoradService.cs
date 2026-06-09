using Lab.Accounting.API.Common.Requests.Category;

namespace Lab.Accounting.API.Services
{
    public interface IDashBoradService
    {
        /// <summary>
        /// 查看賣家所有數據
        /// </summary>
        /// <param name="sellerUserId">賣家 ID</param>
        /// <returns>賣家數據</returns>
        Task<ApiResponse<DashBoardResponse>> GetDashboard(int sellerUserId);
    }
}
