namespace Lab.Accounting.API.Repositories;

public class ProductsImgRepository(DBConnecting connecting) : IProductsImgRepository
{
    /// <summary>
    /// 商品圖片上傳
    /// </summary>
    /// <param name="productsImgs">圖片</param>
    /// <param name="reviewId">審查表 ID</param>
    /// <returns>影響列數</returns>
    public async Task<int> ProductsImgUpload(int reviewId, string productsImgs)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Insert into
                  ProductImg
                  (ProductsReviewId, ProductsImg)
                  Values
                  (@ProductsReviewId, @ProductsImg)
                ";

        return await conn.ExecuteAsync(sql, new { ProductsReviewId = reviewId, ProductsImg = productsImgs });
    }

    /// <summary>
    /// 查看審查表所有圖片
    /// </summary>
    /// <param name="reviewId">審查表 ID </param>
    /// <returns>商品圖片 URL</returns>
    public async Task<IEnumerable<MallProductImg>> GetReviewAllImg(int reviewId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT *
                FROM   ProductImg
                WHERE  ProductsReviewId = @ProductsReviewId ";

        return await conn.QueryAsync<MallProductImg>(sql, new { ProductsReviewId = reviewId });
    }

    /// <summary>
    /// 查看商品所有圖片
    /// </summary>
    /// <param name="productsId">商品 ID </param>
    /// <returns>商品圖片 URL</returns>
    public async Task<IEnumerable<MallProductImg>> GetProductsAllImg(int productsId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT *
                FROM   ProductImg
                WHERE  ProductsId = @ProductsId ";

        return await conn.QueryAsync<MallProductImg>(sql, new { ProductsId = productsId });
    }

    /// <summary>
    /// 查看商品圖片
    /// </summary>
    /// <param name="productsImgId">商品圖片 ID</param>
    /// <returns>商品圖片 URL</returns>
    public async Task<MallProductImg> GetProductsImg(int productsImgId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT *
                FROM   ProductImg
                WHERE  productsImgId = @productsImgId ";

        return await conn.QueryFirstOrDefaultAsync<MallProductImg>(sql, new { productsImgId = productsImgId });
    }

    /// <summary>
    /// 審核通過後新增圖片的商品 ID
    /// </summary>
    /// <param name="reviewId">審查表 ID</param>
    ///  <param name="productsId">商品 ID</param>
    /// <returns>刪除的圖片</returns>
    public async Task<int> UpdateImgsToProductId(int reviewId, int productsId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Update
                    ProductImg
                    Set ProductsId=COALESCE(@ProductsId, ProductsId)
                     Where ProductsReviewId=@ProductsReviewId";

        return await conn.ExecuteAsync(sql, new { ProductsReviewId = reviewId, ProductsId = productsId });
    }

    /// <summary>
    /// 刪除商品圖片
    /// </summary>
    /// <param name="productsImgId">商品圖片 ID</param>
    /// <returns>刪除的圖片</returns>
    public async Task<MallProductImg> DeleteProductsImg(int productsImgId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"Delete 
                FROM   ProductImg 
                Output [DELETED].*
                WHERE  ProductsImgId = @ProductsImgId ";

        return await conn.QueryFirstOrDefaultAsync<MallProductImg>(sql, new { ProductsImgId = productsImgId });
    }
}
