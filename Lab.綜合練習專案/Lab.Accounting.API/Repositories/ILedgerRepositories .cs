namespace Lab.Accounting.API.Repositories;

public interface ILedgerRepositories
{
    /// <summary>
    /// 查看單一帳本項目
    /// </summary>
    /// <param name="ledgerId">項目名稱</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>單筆項目</returns>
    Task<LedgerItemJoinCategoryView> GetLedger(int ledgerId,int userId);

    /// <summary>
    /// 查看全部帳本項目
    /// </summary>
    /// <param name="categoryId">項目類別</param>
    ///  <param name="date">日期</param>
    ///  <param name="itemname">項目名稱</param>
    ///  <param name="userId">使用者 ID</param>
    /// <returns>所有項目</returns>
    Task<List<LedgerItemJoinCategoryView>> GetAllLedger(
        List<int>? categoryId,
        DateTime? date,
        string? itemname,
        int userId
    );

    /// <summary>
    /// 新增帳本項目
    /// </summary>
    /// <param name="insert">新增帳本項目所有細項</param>
    /// <returns>新增的帳本項目</returns>
    Task<int> CreateLedger(LedgerItem insert);

    /// <summary>
    /// 更新指定帳本項目
    /// </summary>
    /// <param name="update">更新帳本項目所有細項</param>
    /// <returns>影響列數</returns>
    Task<int> UpdateLedger(LedgerItem update);

    /// <summary>
    /// 刪除指定帳本項目
    /// </summary>
    /// <param name="ledgerId">項目 ID</param>
    /// <param name="isDelete">刪除狀態</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>影響列數</returns>
    Task<int> DeleteLedger(int ledgerId, bool isDelete, int userId);
}
