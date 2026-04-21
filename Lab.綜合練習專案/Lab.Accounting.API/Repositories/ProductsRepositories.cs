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
                                 m.ProductsStock,
                                 m.isDelete,
                                 STRING_AGG(c.productcategoryname, ',') as Productcategoryname
                        FROM     mallproducts m
                        JOIN     productcategory p
                        ON       m.productsid=p.productsid
                        JOIN     mallproductcategory c
                        ON       c.productcategoryid=p.productcategoryid
                        GROUP BY 
                               m.productsid,
                               m.userid,
                               m.productsname,
                               m.productsprice,
                               m.isDelete,
                               m.ProductsStock
                        ORDER BY productsid offset 0 rows FETCH next 10 rows only";

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
                               m.ProductsStock,
                               m.isDelete,
                               STRING_AGG(c.productcategoryname, ',') as Productcategoryname
                        FROM   mallproducts m
                               left JOIN productcategory p
                                 ON m.productsid = p.productsid
                               left JOIN mallproductcategory c
                                 ON c.productcategoryid = p.productcategoryid
                        WHERE  m.ProductsId = @ProductsId
                               AND m.UserId = @UserId
                        GROUP BY 
                               m.productsid,
                               m.userid,
                               m.productsname,
                               m.productsprice,
                               m.isDelete,
                               m.ProductsStock";

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
                                     productsprice,
                                     ProductsStock,
                                     IsDelete
                                     )
                        VALUES      (@UserId,
                                     @ProductsName,
                                     @ProductsPrice,
                                     @ProductsStock,
                                     @IsDelete
                                     ) 
                        Select 
                                    Cast(
                                    Scope_Identity() as int
                                    );";
            return await conn.QuerySingleAsync<int>(sql, products);
        }

        /// <summary>
        /// 更新單一商品
        /// </summary>
        /// <param name="products">商品資訊</param>
        /// <returns>影響列數</returns>
        public async Task<int> UpdateProducts(MallProducts products)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"UPDATE mallproducts
                        SET      
                                 productsname = @ProductsName,
                                 productsprice = @ProductsPrice,
                                 ProductsStock = @ProductsStock,
                        WHERE    productsid = @ProductsId and userId=@UserId;";
            return await conn.ExecuteAsync(sql, products);
        }

        /// <summary>
        /// 軟刪除或硬刪除單一商品
        /// </summary>
        /// <param name="productsId">商品 ID</param>
        /// <param name="isDelete">刪除狀態</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>影響列數</returns>
        public async Task<int> DeleteProducts(int productsId, bool isDelete, int userId)
        {
            using var conn = connecting.CreateConnecting();
            //用 true 跟 false 判斷是否執行硬刪除或是軟刪除
            var deletesql = isDelete
                ? @"
                    Delete From ProductImg Where ProductsID=@productsId;
                    Delete From MallProductsRate Where ProductsID=@productsId;
                    Delete From MallShoppingCar Where ProductsID=@productsId;
                    Delete From ProductCategory Where ProductsID=@productsId;
                    Delete From MallProducts Where ProductsID=@productsId and UserId=@userId;"
                : @"Update MallProducts Set IsDelete=1 Where ProductsID=@productsId and UserId=@userId;
                    ";

            return await conn.ExecuteAsync(deletesql, new { productsId = productsId, userId = userId });
        }

        /// <summary>
        /// 設定商品庫存
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="purchaseQuantity">購買數量</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        public async Task<int> SetStock(int productsId, int purchaseQuantity, int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Update mallproducts
                    SET ProductsStock = @purchaseQuantity
                    WHERE ProductsId = @productsId and UserId = @UserId";

            return await conn.ExecuteAsync(
                sql,
                new
                {
                    productsId,
                    purchaseQuantity,
                    userId,
                }
            );
        }

        /// <summary>
        /// 購買商品
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        public async Task<int> BuyProducts(int productsId, int userId)
        {
            using var conn = connecting.CreateConnecting();

            var addBoughtProductsql =
                @"INSERT INTO MallBoughtProducts
                            (userid,
                            productsid,
                            BoughtTIme)
                VALUES     (@UserId,
                            @ProductsId,
                            @BoughtTIme)";

            return await conn.ExecuteAsync(
                addBoughtProductsql,
                new
                {
                    UserId = userId,
                    ProductsId = productsId,
                    BoughtTIme = DateTime.Now,
                }
            );
        }
    }
}
