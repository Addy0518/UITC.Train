using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;

namespace Lab.Accounting.API.Repositories
{
    public class ProductsImgRepository(DBConnecting connecting) : IProductsImgRepository
    {
        /// <summary>
        /// 商品圖片上傳
        /// </summary>
        /// <param name="productImgs">多個商品圖片檔案</param>
        /// <returns>影響列數</returns>
        public async Task<int> ProductsImgUpload(IEnumerable<ProductImg> productImgs)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Insert into
                  ProductImg
                  (ProductsId, ProductsImg),
                  Values
                  (@ProductsId, @ProductsImg)
                ";

            return await conn.ExecuteAsync(sql, productImgs);
        }

        /// <summary>
        /// 查看商品圖片
        /// </summary>
        /// <param name="productsId">商品 ID </param>
        /// <returns>商品圖片 URL</returns>
        public async Task<IEnumerable<string>> GetProductsImg(int productsId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT productsimg
                FROM   productimg
                WHERE  productsid = @ProductsId ";

            return await conn.QueryAsync<string>(sql, new { ProductsId = productsId });
        }
    }
}
