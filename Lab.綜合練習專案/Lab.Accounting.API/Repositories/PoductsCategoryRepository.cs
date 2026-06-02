using NPOI.HPSF;
using NPOI.POIFS.NIO;
using NPOI.POIFS.Properties;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab.Accounting.API.Repositories
{
    public class PoductsCategoryRepository(DBConnecting connecting) : IPoductsCategoryRepository
    {
        /// <summary>
        /// 查看指定類別
        /// </summary>
        /// <param name="categoryId">商品類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<MallProductCategory> GetCategories(int categoryId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"select *
                from MallProductCategory 
                where ProductCategoryId=@categoryId";
            return await conn.QueryFirstOrDefaultAsync<MallProductCategory>(sql, new { categoryId = categoryId });
        }

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
        /// 查看最頂層一層的父類別
        /// </summary>
        /// <returns>商品類別</returns>
        public async Task<IEnumerable<MallProductCategory>> GetOneFatherCategory()
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"select c.*
                from MallProductCategory c
                Join MallProductCategory_Closure cl on cl.SonId=c.ProductCategoryId
                where cl.FatherId=cl.SonId
                and cl.Depth=0
                and c.ProductParentId is null";
            return await conn.QueryAsync<MallProductCategory>(sql);
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
        /// <param name = "request" > 類別新增資訊 </param >
        /// <returns>新增的類別 ID </returns>
        public async Task<int> AddCategory(CategoryInsertRequest request)
        {
            using var conn = connecting.CreateConnecting();
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 新增類別
                    var sql1 =
                        @" INSERT INTO MallProductCategory (ProductCategoryName, ProductParentId)
                         VALUES (@ProductCategoryName, @ProductParentId);
                         SELECT SCOPE_IDENTITY();";
                    var sonId = await conn.ExecuteScalarAsync<int>(sql1, request);

                    // 用剛剛新增的類別的 ID 新增類別關聯閉鎖表
                    var sql2 =
                        @" INSERT INTO MallProductCategory_Closure (FatherId, SonId, Depth)
                           VALUES (@SonId, @SonId, 0)";
                    await conn.ExecuteAsync(sql2, new { SonId = sonId });

                    // 如果他不是最高層級就在新增他的父類別關聯
                    // 比如 : Closure 裡 SonId = ParentId ( 比如是 39 ) 的資料：
                    // 39 | 39 | 0    ← 自己
                    // 38 | 39 | 1    ← 38 是 39 的父
                    // 37 | 39 | 2    ← 37 是 39 的祖父

                    // 然後 就全部 + 1 ( 因為又多下一層 ( SonId = 41 )) , 變成=>
                    // 39, 39, 0  → 變成(39, 41, 1)
                    // 38, 39, 1  → 變成(38, 41, 2)
                    // 37, 39, 2  → 變成(37, 41, 3)
                    if (request.ProductParentId.HasValue)
                    {
                        var sql3 =
                            @" INSERT INTO MallProductCategory_Closure (FatherId, SonId, Depth)
                             SELECT FatherId, @SonId, Depth + 1
                             FROM MallProductCategory_Closure
                             WHERE SonId = @ParentId;";
                        await conn.ExecuteAsync(sql3, new { SonId = sonId, ParentId = request.ProductParentId });
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
        /// 新增類別圖片
        /// </summary>
        /// <param name = "categoryId" > 商品類別 ID </param >
        /// <param name = "fileName" > 檔案名稱 </param >
        /// <returns>影響列數 </returns>
        public async Task<int> UploadCategoryImg(int categoryId, string fileName)
        {
            using var conn = connecting.CreateConnecting();

            var sql1 =
                @"UPDATE MallProductCategory 
                  SET ProductCategoryImg = @FileName 
                  WHERE ProductCategoryId = @CategoryId";
            return await conn.ExecuteAsync(sql1, new { FileName = fileName, CategoryId = categoryId });
        }

        /// <summary>
        /// 刪除類別及關連閉鎖表
        /// </summary>
        /// <param name="categoryId">類別 ID </param>
        /// <returns>刪除的類別資訊 </returns>
        public async Task<IEnumerable<MallProductCategory>> DeleteCategory(int categoryId)
        {
            using var conn = connecting.CreateConnecting();
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // 刪除類別
                    var sql =
                        @"Delete from MallProductCategory OUTPUT [DELETED].* Where ProductCategoryId IN (
                                SELECT SonId FROM MallProductCategory_Closure 
                                WHERE FatherId = @CategoryId)";
                    var affectRows = await conn.QueryAsync<MallProductCategory>(sql, new { CategoryId = categoryId });

                    // 刪除閉鎖表的關聯 ( 除了刪除自己以外也刪除底下的其他類別 , 不然會有孤兒類別 )
                    var sql2 =
                        @" Delete from MallProductCategory_Closure 
                           Where SonId in (Select SonId From MallProductCategory_Closure Where FatherId=@CategoryId)";
                    await conn.ExecuteAsync(sql2, new { CategoryId = categoryId });

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
