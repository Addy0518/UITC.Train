namespace Lab.Accounting.API.Repositories;

public class LedgerRepositories(DBConnecting connecting) : ILedgerRepositories
{
    /// <summary>
    /// 查看單一帳本項目
    /// </summary>
    /// <param name="ledgerId">項目名稱</param>
    /// <returns>單筆項目</returns>
    public async Task<LedgerItemJoinCategoryView> GetLedger(int ledgerId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Select 
                      *, 
                      c.CategoryName 
                    From 
                      LedgerItem l 
                      join LedgerItemCategory c on c.CategoryId = l.CategoryId 
                    where 
                      ItemId = @ledgerId
                    ";

        var result = await conn.QuerySingleAsync<LedgerItemJoinCategoryView>(
            sql,
            new { ledgerId = ledgerId }
        );

        return result;
    }

    /// <summary>
    /// 查看全部帳本項目
    /// </summary>
    ///  <param name="categoryId">項目類別</param>
    ///  <param name="date">日期</param>
    ///  <param name="itemname">項目名稱</param>
    /// <returns>所有項目</returns>
    public async Task<List<LedgerItemJoinCategoryView>> GetAllLedger(
        List<int>? categoryId,
        DateTime? date,
        string? itemname
    )
    {
        using var conn = connecting.CreateConnecting();

        // 沒帶參數的查詢 , 用 where 1=1 來讓後面可以動態街上其他查詢參數
        var sql =
            @"Select 
                      *, 
                      c.CategoryName 
                    From 
                      LedgerItem l 
                      join LedgerItemCategory c on c.CategoryId = l.CategoryId 
                    where 
                      1 = 1";
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

        var result = await conn.QueryAsync<LedgerItemJoinCategoryView>(sql, parm);
        return result.ToList();
    }

    /// <summary>
    /// 新增帳本項目
    /// </summary>
    /// <param name="insert">新增帳本項目所有細項</param>
    /// <returns>新增的帳本項目</returns>
    public async Task<int> CreateLedger(LedgerItem insert)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"Insert Into LedgerItem (
                  ItemName, ItemCost, CategoryId, ItemCreateDate, 
                  UserId, ItemIllustrate, IsDelete
                ) 
                values 
                  (
                    @ItemName, @ItemCost, @CategoryId, 
                    @ItemCreateDate, @UserId, @ItemIllustrate, 
                    @IsDelete
                  );
                Select 
                  Cast(
                    Scope_Identity() as int
                  );";

        return await conn.QuerySingleAsync<int>(sql, insert);
    }

    /// <summary>
    /// 更新指定帳本項目
    /// </summary>
    /// <param name="update">更新帳本項目所有細項</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateLedger(LedgerItem update)
    {
        using var conn = connecting.CreateConnecting();
        //throw new Exception("錯誤拉!");
        // 這裡我用 COALESCE 來確保使用者沒輸入的話就保持原樣
        var sql =
            @"Update 
                  LedgerItem 
                Set 
                  ItemName = COALESCE(@ItemName, ItemName), 
                  ItemCost = COALESCE(@ItemCost, ItemCost), 
                  CategoryId = COALESCE(@CategoryId, CategoryId), 
                  ItemUpdateDate = COALESCE(@ItemUpdateDate, ItemUpdateDate), 
                  IsDelete = COALESCE(@IsDelete, IsDelete), 
                  ItemIllustrate = COALESCE(@ItemIllustrate, ItemIllustrate) 
                where 
                  ItemId = @ItemId";

        return await conn.ExecuteAsync(sql, update);
    }

    /// <summary>
    /// 刪除指定帳本項目
    /// </summary>
    /// <param name="ledgerId">項目 ID</param>
    /// <param name="isDelete">刪除狀態</param>
    /// <returns>影響列數</returns>
    public async Task<int> DeleteLedger(int ledgerId, bool isDelete)
    {
        using var conn = connecting.CreateConnecting();
        //用 true 跟 false 判斷是否執行硬刪除或是軟刪除
        var deletesql = isDelete
            ? @"Delete From LedgerItem Where ItemId=@ledgerId"
            : @"Update LedgerItem Set IsDelete=1 Where ItemId=@ledgerId";

        return await conn.ExecuteAsync(deletesql, new { ledgerId = ledgerId });
    }
}
