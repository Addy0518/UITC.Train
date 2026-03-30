namespace Lab.Accounting.API.Services
{
    public class LedgerService(
        ILedgerRepositories accountrepo,
        ILedgerItemCategoryRepositories categoryrepo,
        DBConnecting connecting
    ) : ILedgerService
    {
        /// <summary>
        /// 查看全部帳本項目
        /// </summary>
        /// /// <param name="categoryId">項目類別</param>
        ///  <param name="date">日期</param>
        ///  <param name="itemname">項目名稱</param>
        /// <returns>所有項目</returns>
        public async Task<ApiResponse<List<LedgerItemJoinCategoryView>>> GetAllLedger(
            List<int>? categoryId,
            DateTime? date,
            string? itemname
        )
        {
            return ApiResponseHelper.Success(
                await accountrepo.GetAllLedger(categoryId, date, itemname),
                "成功!"
            );
        }

        /// <summary>
        /// 查看單一帳本項目
        /// </summary>
        /// <param name="ledgerId">項目名稱</param>
        /// <returns>單筆項目</returns>
        public async Task<ApiResponse<LedgerItemJoinCategoryView>> GetLedger(int ledgerId)
        {
            var target = await accountrepo.GetLedger(ledgerId);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<LedgerItemJoinCategoryView>();
            }
            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 新增帳本項目
        /// </summary>
        /// <param name="insert">新增帳本項目所有細項</param>
        /// <returns>新增的帳本項目</returns>
        public async Task<ApiResponse<int>> CreateLedger(LedgerInsertRequest insert)
        {
            using var conn = connecting.CreateConnecting();
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int categoryId = await CategoryExistCreate(insert.CategoryName);
                var result = new LedgerItem
                {
                    CategoryId = categoryId,
                    ItemName = insert.ItemName,
                    ItemCost = insert.ItemCost,
                    ItemCreateDate = insert.ItemCreateDate ?? DateTime.Now,
                    ItemUpdateDate = DateTime.Now,
                    ItemIllustrate = insert.ItemIllustrate,
                    // User 這邊要改
                    UserId = 1,
                    IsDelete = false,
                };

                var done = await accountrepo.CreateLedger(result);

                trxScope.Complete();

                return ApiResponseHelper.Success(done, "成功!");
            }
        }

        /// <summary>
        /// 更新指定帳本項目
        /// </summary>
        /// <param name="update">更新帳本項目所有細項</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UpdateLedger(LedgerUpdateRequest update)
        {
            using var conn = connecting.CreateConnecting();
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                int categoryId = await CategoryExistCreate(update.CategoryName);

                var result = new LedgerItem
                {
                    CategoryId = categoryId,
                    ItemId = update.ItemId,
                    ItemName = update.ItemName,
                    ItemCost = update.ItemCost,
                    ItemUpdateDate = DateTime.Now,
                    ItemIllustrate = update.ItemIllustrate,
                    // User 這邊要改
                    UserId = 1,
                    IsDelete = false,
                };

                var target = await accountrepo.UpdateLedger(result);

                if (target == null)
                {
                    return ApiResponseHelper.NotFound<int>();
                }

                trxScope.Complete();

                return ApiResponseHelper.Success<int>(target, "成功!");
            }
        }

        /// <summary>
        /// 新增或是查看類別
        /// </summary>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>類別 ID </returns>
        private async Task<int> CategoryExistCreate(string categoryname)
        {
            int existcategory = 0;
            if (!string.IsNullOrWhiteSpace(categoryname))
            {
                existcategory = await categoryrepo.GetLedgerItemCategory(categoryname);
            }

            int categoryId = 0;
            if (existcategory > 0)
            {
                categoryId = existcategory;
            }
            else
            {
                categoryId = await categoryrepo.CreateLedgerItemCategory(categoryname);
            }

            return categoryId;
        }

        /// <summary>
        /// 刪除指定帳本項目
        /// </summary>
        /// <param name="ledgerId">項目 ID</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> DeleteLedger(int ledgerId)
        {
            var target = await accountrepo.GetLedger(ledgerId);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            var deletetarget = await accountrepo.DeleteLedger(ledgerId, target.IsDelete);

            return ApiResponseHelper.Success<int>(deletetarget, "成功!");
        }
    }
}
