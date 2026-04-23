using System.Globalization;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class ProductsCategoryRepositories(DBConnecting connecting) : IProductsCategoryRepositories
    {
        /// <summary>
        /// 查看商品類別
        /// </summary>
        /// <param name="productcategoryId">商品類別 ID</param>
        /// <returns>商品類別 ID</returns>
        public async Task<int> GetCategory(int? productcategoryId = null)
        {
            using var conn = connecting.CreateConnecting();

            //  第一層 ProductCategoryId 跟 ProductParentId 為 null 的是最頂層的類別
            //  第二層 ( 子 ) ProductParentId = ( 父 ) ProductCategoryId  的就是往下一層的類別
            //  (衣服 => 短袖 , 長袖 => 男士短袖 , 女士短袖 ...)
            var sql =
                @"Select ProductCategoryId,ProductCategoryName,ProductParentId 
                From mallproductcategory 
                Where (@ProductCategoryId is null and ProductParentId is null) 
                OR ProductParentId = @ProductCategoryId";
            return await conn.QueryFirstOrDefaultAsync<int>(sql, new { ProductCategoryId = productcategoryId });
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
