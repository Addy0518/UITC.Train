using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class ProductsImgRepository(DBConnecting connecting) : IProductsImgRepository
    {
        /// <summary>
        /// 商品圖片上傳
        /// </summary>
        /// <param name="productsImgs">圖片</param>
        /// <param name="productId">商品 ID</param>
        /// <returns>影響列數</returns>
        public async Task<int> ProductsImgUpload(int productId, string productsImgs)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Insert into
                  ProductImg
                  (ProductsId, ProductsImg)
                  Values
                  (@ProductsId, @ProductsImg)
                ";

            return await conn.ExecuteAsync(sql, new { ProductsId = productId, ProductsImg = productsImgs });
        }

        /// <summary>
        /// 查看商品所有圖片
        /// </summary>
        /// <param name="productsId">商品 ID </param>
        /// <returns>商品圖片 URL</returns>
        public async Task<IEnumerable<ProductImg>> GetProductsAllImg(int productsId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT *
                FROM   ProductImg
                WHERE  ProductsId = @ProductsId ";

            return await conn.QueryAsync<ProductImg>(sql, new { ProductsId = productsId });
        }

        /// <summary>
        /// 查看商品圖片
        /// </summary>
        /// <param name="productsImgId">商品圖片 ID</param>
        /// <returns>商品圖片 URL</returns>
        public async Task<ProductImg> GetProductsImg(int productsImgId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT *
                FROM   ProductImg
                WHERE  productsImgId = @productsImgId ";

            return await conn.QueryFirstOrDefaultAsync<ProductImg>(sql, new { productsImgId = productsImgId });
        }

        /// <summary>
        /// 刪除商品圖片
        /// </summary>
        /// <param name="productsImgId">商品圖片 ID</param>
        /// <returns>影響列數</returns>
        public async Task<int> DeleteProductsImg(int productsImgId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Delete 
                FROM   ProductImg
                WHERE  ProductsImgId = @ProductsImgId ";

            return await conn.ExecuteAsync(sql, new { ProductsImgId = productsImgId });
        }
    }
}
