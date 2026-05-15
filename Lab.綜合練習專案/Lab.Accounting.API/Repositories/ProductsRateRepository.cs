namespace Lab.Accounting.API.Repositories.Interface;

public class ProductsRateRepository(DBConnecting connecting) : IProductsRateRepository
{
    /// <summary>
    /// 新增單一商品評價
    /// </summary>
    /// <param name="productrate">商品評價資訊</param>
    /// <returns>影響列數</returns>
    public async Task<int> CreateProductRate(MallProductsRate productrate)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"INSERT INTO MallProductsRate
                                    (UserId,
                                     ProductsId,
                                     Rating,
                                     Comment,
                                     CreateTime)
                        VALUES      (@UserId,
                                     @ProductsId,
                                     @Rating,
                                     @Comment,
                                     @CreateTime) 
                        Select 
                                    Cast(
                                    Scope_Identity() as int
                                    );";
        return await conn.QuerySingleAsync<int>(
            sql,
            new
            {
                UserId = productrate.UserId,
                ProductsId = productrate.ProductsId,
                Rating = productrate.Rating,
                Comment = productrate.Comment,
                CreateTime = productrate.CreateTime,
            }
        );
    }

    /// <summary>
    /// 刪除單一商品評價
    /// </summary>
    /// <param name="productRateId">商品評價 ID</param>
    /// <returns>影響列數</returns>
    public async Task<int> DeleteProductRate(int productRateId)
    {
        using var conn = connecting.CreateConnecting();

        var sql = @"Delete From MallProductsRate Where ProductsRateId=@ProductsRateId";
        return await conn.ExecuteAsync(sql, new { ProductsRateId = productRateId });
    }

    /// <summary>
    /// 查看單一商品評價
    /// </summary>
    /// <param name="productId">商品 ID</param>
    /// <returns>商品評價資訊</returns>
    public async Task<IEnumerable<RateResponse>> GetProductRate(int productId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT   
                       u.UserName,        
                       u.UserHeadshot,
                       r.Rating,
                       r.Comment,
                       r.CreateTime
                FROM   MallProductsRate r
                Join   [User] u on r.UserId=u.UserId
                WHERE  r.ProductsId = @ProductsId ";

        return await conn.QueryAsync<RateResponse>(sql, new { ProductsId = productId });
    }

    /// <summary>
    /// 計算商品評分平均值
    /// </summary>
    /// <param name="productId">商品 ID</param>
    /// <returns>評分平均值</returns>
    public async Task<decimal?> CountAVGProductRate(int productId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT Round(Avg(rating),1)
                FROM   mallproductsrate
                WHERE  productsid = @productsId ";

        return await conn.QuerySingleAsync<decimal?>(sql, new { productsId = productId });
    }
}
