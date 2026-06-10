namespace Lab.Accounting.API.Repositories.Interface;

public class ProductsRateRepository(DBConnecting connecting) : IProductsRateRepository
{
    /// <summary>
    /// 新增單一商品評價
    /// </summary>
    /// <param name="productrate">商品評價資訊</param>
    /// <returns>影響列數</returns>
    public async Task<int> CreateProductRate(ProductRate productrate)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"INSERT INTO ProductRate
                                    (UserId,
                                     ProductsId,
                                     OrderId,
                                     Rating,
                                     Comment,
                                     CreateTime)
                        VALUES      (@UserId,
                                     @ProductsId,
                                     @OrderId,
                                     @Rating,
                                     @Comment,
                                     @CreateTime) 
                        Select 
                                    Cast(
                                    Scope_Identity() as int
                                    );";
        return await conn.QuerySingleAsync<int>(sql, productrate);
    }

    /// <summary>
    /// 刪除單一商品評價
    /// </summary>
    /// <param name="productRateId">商品評價 ID</param>
    /// <returns>影響列數</returns>
    public async Task<int> DeleteProductRate(int productRateId)
    {
        using var conn = connecting.CreateConnecting();

        var sql = @"Delete From ProductRate Where ProductsRateId=@ProductsRateId";
        return await conn.ExecuteAsync(sql, new { ProductsRateId = productRateId });
    }

    /// <summary>
    /// 查看單一訂單評價
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>商品評價資訊</returns>
    public async Task<RateResponse> GetOrderRate(int orderId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT   Top 1
                       u.UserName,        
                       u.UserHeadshot,
                       r.OrderId,
                       r.Rating,
                       r.Comment,
                       r.CreateTime
                FROM   ProductRate r
                Join   [User] u on r.UserId=u.UserId
                WHERE  r.OrderId = @OrderId ";

        return await conn.QueryFirstOrDefaultAsync<RateResponse>(sql, new { OrderId = orderId });
    }

    /// <summary>
    /// 查看單一商品所有評價
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
                       r.OrderId,
                       r.Rating,
                       r.Comment,
                       r.CreateTime
                FROM   ProductRate r
                Join   [User] u on r.UserId=u.UserId
                WHERE  r.ProductsId = @ProductsId ";

        return await conn.QueryAsync<RateResponse>(sql, new { ProductsId = productId });
    }

    /// <summary>
    /// 查看賣家所有商品所有評價的數量
    /// </summary>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>商品評價資訊</returns>
    public async Task<int> GetAllProductsRateCount(int sellerId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT   Count(*)
                FROM   ProductRate r
                Join   Product p 
                on     p.ProductsId=r.ProductsId
                WHERE  p.UserId = @SellerId";

        return await conn.ExecuteScalarAsync<int>(sql, new { SellerId = sellerId });
    }

    /// <summary>
    /// 計算賣家單一商品評分平均值
    /// </summary>
    /// <param name="productId">商品 ID</param>
    /// <returns>評分平均值</returns>
    public async Task<decimal?> CountAVGProductRate(int productId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT Round(Avg(rating),1)
                FROM   productrate
                WHERE  productsid = @productsId ";

        return await conn.QuerySingleAsync<decimal?>(sql, new { productsId = productId });
    }

    /// <summary>
    /// 計算賣家所有商品評分平均值
    /// </summary>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>評分平均值</returns>
    public async Task<decimal?> CountAVGAllProductRate(int sellerId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT Round(Avg(rating),1)
                FROM   productrate r
                Join   Product p 
                on     p.ProductsId=r.ProductsId
                WHERE  p.UserId = @SellerId ";

        return await conn.QuerySingleAsync<decimal?>(sql, new { SellerId = sellerId });
    }
}
