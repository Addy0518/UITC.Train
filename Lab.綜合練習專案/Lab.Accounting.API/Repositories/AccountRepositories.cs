using Lab.API.TODO.Common.Requests;
using Microsoft.IdentityModel.Tokens;

namespace Lab.Accounting.API.Repositories
{
    public class AccountRepositories(AccountConne connecting) : IAccountRepositories
    {
        /// <summary>
        /// 查看單一帳本項目
        /// </summary>
        /// <param name="ledgerId">項目名稱</param>
        /// <returns>單筆項目</returns>
        public async Task<LedgerItemDTO> GetLedger(int ledgerId)
        {
            using var conn = connecting.CreateConnec();

            var sql = @"Select *, c.CategoryName From LedgerItem l join LedgerItemCategory c on c.CategoryId=l.CategoryId where ItemId=@ledgerId";

            var result = await conn.QuerySingleAsync<LedgerItemDTO>(sql, new { ledgerId = ledgerId });

            return result;
        }

        /// <summary>
        /// 查看全部帳本項目
        /// </summary>
        ///  <param name="categoryId">項目類別</param>
        ///  <param name="date">日期</param>
        ///  <param name="itemname">項目名稱</param>
        /// <returns>所有項目</returns>
        public async Task<List<LedgerItemDTO>> GetAllLedger(
            List<int>? categoryId,
            DateTime? date,
            string? itemname
        )
        {
            using var conn = connecting.CreateConnec();

            // 沒帶參數的查詢 , 用 where 1=1 來讓後面可以動態街上其他查詢參數
            var sql = @"Select *, c.CategoryName From LedgerItem l join LedgerItemCategory c on c.CategoryId=l.CategoryId where 1=1";
            var parm = new DynamicParameters();
            //如果有丟入參數就接上查詢 , 要用 In 因為是 List 多筆
            if (categoryId != null && categoryId.Any())
            {
                sql += @" and l.CategoryId in @categoryId";
                parm.Add("categoryId", categoryId);
            }
            if (date.HasValue)
            {
                // 只比對到日 , 小時那些不用
                sql += @" and CAST(ItemCreateDate AS DATE) = @date";
                parm.Add("date", date);
            }
            if (itemname != null && !string.IsNullOrWhiteSpace(itemname))
            {
                sql += @" and ItemName like @itemname";
                parm.Add("itemname", itemname + '%');
            }

            var result = await conn.QueryAsync<LedgerItemDTO>(sql, parm);
            return result.ToList();
        }

        /// <summary>
        /// 新增帳本項目
        /// </summary>
        /// <param name="insert">新增帳本項目所有細項</param> 
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>新增的帳本項目</returns>
        public async Task<int> CreateLedger(LedgerInsertRequest insert, string categoryname)
        {
            using var conn = connecting.CreateConnec();

            var categorysql =
                //用 Exists 檢查如果至少有一個類別就返回 true , 回傳這個現有類別的 ID , 沒有就 false , 直接新增一個
                @"If Exists (Select * from LedgerItemCategory Where CategoryName=@categoryname) 
                              Begin
                              Select CategoryId From LedgerItemCategory Where CategoryName=@categoryname 
                              end
                              else
                              begin
                              Insert Into LedgerItemCategory(CategoryName) Values (@categoryname) Select Cast(Scope_Identity() as int)
                              end";
            int categoryId = await conn.QuerySingleAsync<int>(
                categorysql,
                new { CategoryName = categoryname }
            );

            var sql =
                "Insert Into LedgerItem (ItemName,ItemCost,CategoryId,ItemCreateDate,UserId,ItemIllustrate) values(@ItemName,@ItemCost,@CategoryId,@ItemCreateDate,@UserId,@ItemIllustrate) Select Cast(Scope_Identity() as int);";

            var result = new LedgerInsertRequest
            {
                CategoryId = categoryId,
                ItemCost = insert.ItemCost,
                ItemName = insert.ItemName,
                ItemCreateDate = insert.ItemCreateDate??DateTime.Now,
                ItemIllustrate = insert.ItemIllustrate,
                UserId = 1,
                isDelete = false,
            };
            return await conn.QuerySingleAsync<int>(sql, result);
        }

        /// <summary>
        /// 更新指定帳本項目
        /// </summary>
        /// <param name="update">更新帳本項目所有細項</param>
        /// <param name="categoryname">項目類別名稱</param>
        /// <returns>影響列數</returns>
        public async Task<int> UpdateLedger(LedgerUpdateRequest update, string? categoryname)
        {
            using var conn = connecting.CreateConnec();
            //使用者更新不一定會更新類別,所以先給個 null 以防萬一
            int? categoryId = null;
            if (!string.IsNullOrWhiteSpace(categoryname))
            {
                var categorysql =
                    //用 Exists 檢查如果至少有一個類別就返回 true , 回傳這個現有類別的 ID , 沒有就 false , 直接新增一個
                    @"If Exists (Select * from LedgerItemCategory Where CategoryName=@categoryname) 
                              Begin
                              Select CategoryId From LedgerItemCategory Where CategoryName=@categoryname 
                              end
                              else
                              begin
                              Insert Into LedgerItemCategory(CategoryName) Values (@categoryname) Select Cast(Scope_Identity() as int)
                              end";
                categoryId = await conn.QuerySingleAsync<int>(
                    categorysql,
                    new { CategoryName = categoryname }
                );
            }
            // 這裡我 CategoryId 用 COALESCE 來確保使用者沒輸入的話就保持原樣
            var sql =
                "Update LedgerItem Set ItemName=COALESCE(@ItemName,ItemName),ItemCost=COALESCE(@ItemCost,ItemCost),CategoryId=COALESCE(@CategoryId, CategoryId),ItemUpdateDate=COALESCE(@ItemUpdateDate,ItemUpdateDate),IsDelete=COALESCE(@IsDelete,IsDelete),ItemIllustrate=COALESCE(@ItemIllustrate,ItemIllustrate) where ItemId=@ItemId";

            return await conn.ExecuteAsync(
                sql,
                new
                {
                    update.ItemId,
                    update.ItemName,
                    update.ItemCost,
                    update.isDelete,
                    update.ItemIllustrate,
                    CategoryId = categoryId,
                    ItemUpdateDate = update.ItemUpdateDate?? DateTime.Now,
                }
            );
        }

        /// <summary>
        /// 刪除指定帳本項目
        /// </summary>
        /// <param name="ledgerId">項目 ID</param>
        /// <param name="isDelete">刪除狀態</param>
        /// <returns>影響列數</returns>
        public async Task<int> DeleteLedger(int ledgerId, bool isDelete)
        {
            using var conn = connecting.CreateConnec();
            //用 true 跟 false 判斷是否執行硬刪除或是軟刪除
            var deletesql = isDelete
                ? @"Delete From LedgerItem Where ItemId=@ledgerId"
                : @"Update LedgerItem Set IsDelete=1 Where ItemId=@ledgerId";

            return await conn.ExecuteAsync(deletesql, new { ledgerId = ledgerId });
        }
    }
}
