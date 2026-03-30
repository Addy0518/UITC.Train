namespace Lab.Accounting.API.Repositories;

public interface ILedgerItemCategoryRepositories
{
    /// <summary>
    /// 查看帳本類別是否存在資料庫
    /// </summary>
    ///  <param name="categoryname">帳本類別名稱</param>
    /// <returns>帳本類別名稱</returns>
    Task<int> GetLedgerItemCategory(string? categoryname);

    /// <summary>
    /// 新建帳本類別
    /// </summary>
    ///  <param name="categoryname">帳本類別名稱</param>
    /// <returns>帳本類別 ID　</returns>
    Task<int> CreateLedgerItemCategory(string? categoryname);
}
