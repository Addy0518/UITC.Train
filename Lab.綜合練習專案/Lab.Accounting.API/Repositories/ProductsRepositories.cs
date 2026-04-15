using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class ProductsRepositories(DBConnecting connecting) : IProductsRepositories
    {
        /// <summary>
        /// 查看所有商品 ( 分頁 )
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">每頁顯示數量</param>
        /// <returns>商品列表</returns>
        public async Task<IEnumerable<ProductsResponse>> GetAllProducts(int pageIndex, int pageSize)
        {
            using var conn = connecting.CreateConnecting();

            int offset = pageIndex * pageSize;
            // Offset 代表要跳過的行數，Fetch Next 代表要取得的行數
            var sql =
                @"SELECT   m.productsid,
                                 m.userid,
                                 m.productsname,
                                 m.productsprice,
                                 c.productcategoryname
                        FROM     mallproducts m
                        JOIN     productcategory p
                        ON       m.productsid=p.productsid
                        JOIN     mallproductcategory c
                        ON       c.productcategoryid=p.productcategoryid
                        ORDER BY productsid offset @offset rows FETCH next @pageSize rows only";

            var result = await conn.QueryAsync<ProductsResponse>(sql, new { offset = offset, pageSize = pageSize });
            return result;
        }

        /// <summary>
        /// 查看單一商品
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>商品資訊</returns>
        public async Task<ProductsResponse> GetProducts(int productId, int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT m.productsid,
                               m.userid,
                               m.productsname,
                               m.productsprice,
                               c.productcategoryname
                        FROM   mallproducts m
                               JOIN productcategory p
                                 ON m.productsid = p.productsid
                               JOIN mallproductcategory c
                                 ON c.productcategoryid = p.productcategoryid
                        WHERE  m.productsid = @ProductsId
                               AND m.userid = @UserId ";

            var result = await conn.QueryFirstOrDefaultAsync<ProductsResponse>(
                sql,
                new { ProductsId = productId, UserId = userId }
            );

            return result;
        }

        /// <summary>
        /// 新增單一商品
        /// </summary>
        /// <param name="products">商品資訊</param>
        /// <returns>影響列數</returns>
        public async Task<int> CreateProducts(MallProducts products)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO mallproducts
                                    (userid,
                                     productsname,
                                     productsprice)
                        VALUES      (@UserId,
                                     @ProductsName,
                                     @ProductsPrice) 
                        Select 
                                    Cast(
                                    Scope_Identity() as int
                                    );";
            return await conn.QuerySingleAsync<int>(
                sql,
                new
                {
                    UserId = products.UserId,
                    ProductsName = products.ProductsName,
                    ProductsPrice = products.ProductsPrice,
                }
            );
        }
    }
}
