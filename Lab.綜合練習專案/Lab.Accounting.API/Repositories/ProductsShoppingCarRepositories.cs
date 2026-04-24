using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class ProductsShoppingCarRepositories(DBConnecting connecting) : IProductsShoppingCarRepositories
    {
        /// <summary>
        /// 查看購物車中的所有商品
        /// </summary>
        /// <param name="userId">使用者 Id</param>
        /// <returns>購物車中的所有商品</returns>
        public async Task<IEnumerable<ProductsResponse>> GetAllProductsInShoppingCar(int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT         m.productsid, 
                                 m.userid,
                                 m.productsname,
                                 m.productsprice,
                                 m.ProductCategoryId,
                                 STRING_AGG(c.productcategoryname, ',') as Productcategoryname
                        FROM     mallproducts m
                        JOIN     mallproductcategory c
                        ON       c.productcategoryid=m.ProductCategoryId                       
                        JOIN      mallshoppingcar s
                        ON       m.productsid = s.productsid
                WHERE  s.userid = @UserId 
                GROUP BY 
                               m.productsid,
                               m.userid,
                               m.productsname,
                               m.ProductCategoryId,
                               m.productsprice";

            return await conn.QueryAsync<ProductsResponse>(sql, new { UserId = userId });
        }

        /// <summary>
        /// 新增單一商品到購物車
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        public async Task<int> AddProductsInShoppingCar(int productsId, int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO mallshoppingcar
                            (userid,
                                productsid)
                SELECT @userid,@productsid
                        
                WHERE  NOT EXISTS (SELECT 1
                                    FROM   mallshoppingcar
                                    WHERE  productsid = @productsid
                                            AND userid = @userid)

                SELECT Cast(@@RowCount AS INT) ;";

            return await conn.QuerySingleAsync<int>(sql, new { UserId = userId, ProductsId = productsId });
        }

        /// <summary>
        /// 刪除單一商品從購物車
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        public async Task<int> DeleteProductsInShoppingCar(int productsId, int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Delete From mallshoppingcar
                Where productsid = @ProductsId and userid = @UserId";

            return await conn.ExecuteAsync(sql, new { ProductsId = productsId, UserId = userId });
        }
    }
}
