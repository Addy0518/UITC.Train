using System.Globalization;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class ProductsCategoryRepositories(DBConnecting connecting) : IProductsCategoryRepositories
    {
        /// <summary>
        /// 查看商品類別
        /// </summary>
        /// <param name="productcategory">商品類別</param>
        /// <returns>影響列數</returns>
        public async Task<int> GetCategory(string productcategory)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select ProductCategoryId From mallproductcategory Where ProductCategoryName = @ProductCategoryName";
            return await conn.QueryFirstOrDefaultAsync<int>(sql, new { ProductCategoryName = productcategory });
        }

        /// <summary>
        /// 新增單一商品類別
        /// </summary>
        /// <param name="productcategory">商品類別</param>
        /// <returns>影響列數</returns>
        public async Task<int> CreateCategory(string productcategory)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO mallproductcategory
                                    (productcategoryname)
                        VALUES      (@ProductCategoryName) 
                        Select 
                                    Cast(
                                    Scope_Identity() as int
                                    );";
            return await conn.QuerySingleAsync<int>(sql, new { ProductCategoryName = productcategory });
        }

        /// <summary>
        /// 新增商品跟類別關聯
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <param name="productcategoryId">商品類別 Id</param>
        /// <returns>影響列數</returns>
        public async Task<int> CreateProductsCategory(int productId, int productcategoryId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO ProductCategory
                                    (ProductsId,ProductCategoryId)
                        VALUES      (@ProductsId,@ProductCategoryId) ";
            return await conn.ExecuteAsync(sql, new { ProductsId = productId, ProductCategoryId = productcategoryId });
        }

        /// <summary>
        /// 刪除商品跟類別關聯
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <returns>影響列數</returns>
        public async Task<int> DeleteProductsCategory(int productId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"DELETE FROM ProductCategory
                  WHERE ProductsId = @ProductsId";

            return await conn.ExecuteAsync(sql, new { ProductsId = productId });
        }
    }
}
