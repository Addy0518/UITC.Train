using Lab.API.TODO.Common.Requests;

namespace Lab.Accounting.API.Services
{
    public class AccountService(IAccountRepositories repositories) : IAccountService
    {
        /// <summary>
        /// 查看全部帳本項目
        /// </summary>
        /// /// <param name="categoryId">項目類別</param>
        ///  <param name="date">日期</param>
        ///  <param name="categoryname">項目名稱</param>
        /// <returns>所有項目</returns>
        public async Task<ApiResponse<List<LedgerItemDTO>>> GetAllLedger(
            List<int>? categoryId,
            DateTime? date,
            string? itemname
        )
        {
            return ApiResponseHelper.Success(
                await repositories.GetAllLedger(categoryId, date, itemname),
                "成功!"
            );
        }

        /// <summary>
        /// 查看單一帳本項目
        /// </summary>
        /// <param name="ledgerId">項目名稱</param>
        /// <returns>單筆項目</returns>
        public async Task<ApiResponse<LedgerItemDTO>> GetLedger(int ledgerId)
        {
            var target = await repositories.GetLedger(ledgerId);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<LedgerItemDTO>();
            }
            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 新增帳本項目
        /// </summary>
        /// <param name="insert">新增帳本項目所有細項</param>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>新增的帳本項目</returns>
        public async Task<ApiResponse<int>> CreateLedger(
            LedgerInsertRequest insert,
            string categoryname
        )
        {
            return ApiResponseHelper.Success(
                await repositories.CreateLedger(insert, categoryname),
                "成功!"
            );
        }

        /// <summary>
        /// 更新指定帳本項目
        /// </summary>
        /// <param name="update">更新帳本項目所有細項</param>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UpdateLedger(
            LedgerUpdateRequest update,
            string? categoryname
        )
        {
            var target = await repositories.UpdateLedger(update, categoryname);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            return ApiResponseHelper.Success<int>(target, "成功!");
        }

        /// <summary>
        /// 刪除指定帳本項目
        /// </summary>
        /// <param name="ledgerId">項目 ID</param>
        /// <param name="isDelete">刪除狀態</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> DeleteLedger(int ledgerId)
        {
            var target = await repositories.GetLedger(ledgerId);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            var deletetarget = await repositories.DeleteLedger(ledgerId, target.IsDelete);

            return ApiResponseHelper.Success<int>(deletetarget, "成功!");
        }
    }
}
