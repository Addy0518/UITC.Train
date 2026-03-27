using Lab.API.TODO.Common.Requests;

namespace Lab.Accounting.API.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// 查看全部帳本項目
        /// </summary>
        /// /// <param name="categoryId">項目類別</param>
        ///  <param name="date">日期</param>
        ///  <param name="itemname">項目名稱</param>
        /// <returns>所有項目</returns>
        Task<ApiResponse<List<LedgerItem>>> GetAllLedger(
            List<int>? categoryId,
            DateTime? date,
            string? itemname
        );

        /// <summary>
        /// 查看單一帳本項目
        /// </summary>
        /// <param name="ledgerId">項目名稱</param>
        /// <returns>單筆項目</returns>
        Task<ApiResponse<LedgerItem>> GetLedger(int ledgerId);

        /// <summary>
        /// 新增帳本項目
        /// </summary>
        /// <param name="insert">新增帳本項目所有細項</param>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>新增的帳本項目</returns>
        Task<ApiResponse<int>> CreateLedger(LedgerInsertRequest insert, string categoryname);

        /// <summary>
        /// 更新指定帳本項目
        /// </summary>
        /// <param name="update">更新帳本項目所有細項</param>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> UpdateLedger(LedgerUpdateRequest update, string? categoryname);

        /// <summary>
        /// 刪除指定帳本項目
        /// </summary>
        /// <param name="ledgerId">項目 ID</param>
        /// <param name="isDelete">刪除狀態</param>
        /// <returns>影響列數</returns>
        Task<ApiResponse<int>> DeleteLedger(int ledgerId);
    }
}
