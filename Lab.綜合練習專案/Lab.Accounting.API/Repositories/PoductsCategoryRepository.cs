using NPOI.POIFS.Properties;

namespace Lab.Accounting.API.Repositories
{
    public class PoductsCategoryRepository(DBConnecting connecting) : IPoductsCategoryRepository
    {
        /// <summary>
        /// 查看指定類別底下的所有層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<IEnumerable<MallProductCategory>> GetSonCategories(int fatherCategoryId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"select c.*
                from MallProductCategory c
                Join MallProductCategory_Closure cl on cl.SonId=c.ProductCategoryId
                where cl.FatherId=@FatherCategoryId
                and cl.Depth>0";
            return await conn.QueryAsync<MallProductCategory>(sql, new { FatherCategoryId = fatherCategoryId });
        }

        /// <summary>
        /// 查看指定類別往上的所有層級類別
        /// </summary>
        /// <param name="sonCategoryId">商品子類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<IEnumerable<MallProductCategory>> GetFatherCategories(int sonCategoryId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"select c.*
                from MallProductCategory c
                Join MallProductCategory_Closure cl on cl.FatherId=c.ProductCategoryId
                where cl.SonId=@SonCategoryId
                order by cl.Depth desc";
            return await conn.QueryAsync<MallProductCategory>(sql, new { SonCategoryId = sonCategoryId });
        }

        /// <summary>
        /// 查看指定類別往下的第一個層級類別
        /// </summary>
        /// <param name="fatherCategoryId">商品父類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<IEnumerable<MallProductCategory>> GetOneSonCategory(int fatherCategoryId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"select c.*
                from MallProductCategory c
                Join MallProductCategory_Closure cl on cl.SonId=c.ProductCategoryId
                where cl.FatherId=@FatherCategoryId
                and cl.Depth=1";
            return await conn.QueryAsync<MallProductCategory>(sql, new { FatherCategoryId = fatherCategoryId });
        }

        /// <summary>
        /// 新增類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryName">類別名稱</param>
        /// <param name="parentId">父類別 ID </param>
        /// <returns>新增的類別 ID </returns>
        public async Task<int> AddCategory(string categoryName, int? parentId)
        {
            using var conn = connecting.CreateConnecting();
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 新增類別
                    var sql1 =
                        @" INSERT INTO MallProductCategory (ProductCategoryName, ProductParentId)
                         VALUES (@CategoryName, @ParentId);
                         SELECT SCOPE_IDENTITY();";
                    var sonId = await conn.ExecuteScalarAsync<int>(
                        sql1,
                        new { CategoryName = categoryName, ParentId = parentId }
                    );

                    // 用剛剛新增的類別的 ID 新增類別關聯閉鎖表
                    var sql2 =
                        @" INSERT INTO MallProductCategory_Closure (FatherId, SonId, Depth)
                           VALUES (@SonId, @SonId, 0)";
                    await conn.ExecuteScalarAsync(sql2, new { SonId = sonId });

                    // 如果他不是最高層級就在新增他的父類別關聯
                    if (parentId.HasValue)
                    {
                        var sql3 =
                            @" INSERT INTO MallProductCategory_Closure (FatherId, SonId, Depth)
                             SELECT FatherId, @SonId, Depth + 1
                             FROM MallProductCategory_Closure
                             WHERE SonId = @ParentId;";
                        await conn.ExecuteScalarAsync(sql3, new { SonId = sonId, ParentId = parentId });
                    }
                    trxScope.Complete();
                    return sonId;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>新增的類別 ID </returns>
        public async Task<int> DeleteCategory(int categoryId)
        {
            using var conn = connecting.CreateConnecting();
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 刪除閉鎖表的關聯 ( 除了刪除自己以外也刪除底下的其他類別 , 不然會有孤兒類別 )
                    var sql1 =
                        @" Delete from MallProductCategory_Closure 
                           Where SonId in (Select SonId From MallProductCategory_Closure Where FatherId=@CategoryId)";
                    await conn.ExecuteAsync(sql1, new { CategoryId = categoryId });

                    // 刪除類別
                    var sql2 = @"Delete from MallProductCategory Where ProductCategoryId=@CategoryId";
                    var affectRows = await conn.ExecuteAsync(sql2, new { CategoryId = categoryId });

                    trxScope.Complete();
                    return affectRows;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
