using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories;

public class LedgerItemCategoryRepositories(DBConnecting connecting) : ILedgerItemCategoryRepositories
{
    /// <summary>
    /// 查看帳本類別
    /// </summary>
    ///  <param name="categoryname">帳本類別名稱</param>
    /// <returns>帳本類別 ID </returns>
    public async Task<int> GetLedgerItemCategory(string? categoryname)
    {
        using var conn = connecting.CreateConnecting();
        {
            var sql =
                @"Select 
                          CategoryId 
                        From 
                          LedgerItemCategory 
                        Where 
                          CategoryName = @categoryname";

            return await conn.QueryFirstOrDefaultAsync<int>(sql, new { CategoryName = categoryname });
        }
    }

    /// <summary>
    /// 新建帳本類別
    /// </summary>
    ///  <param name="categoryname">帳本類別名稱</param>
    /// <returns>帳本類別 ID　</returns>
    public async Task<int> CreateLedgerItemCategory(string? categoryname)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"Insert Into LedgerItemCategory(CategoryName) 
                        Values 
                          (@categoryname) 
                        Select 
                          Cast(
                            Scope_Identity() as int
                          )
                        ";

        return await conn.QuerySingleAsync<int>(sql, new { CategoryName = categoryname });
    }
}
