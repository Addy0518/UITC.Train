namespace Lab.Accounting.API.Repositories;

public class ProductsBuyRepository(DBConnecting connecting) : IProductsBuyRepository
{
    /// <summary>
    /// 查看單一訂單 ( Id 查詢 )
    /// </summary>
    /// <param name="orderId">購買資訊</param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<MallOrder> GetOrder(int orderId, int userId)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Select * From MallOrder
                  Where OrderId = @OrderId
                  And UserId = @UserId";

        return await conn.QueryFirstOrDefaultAsync<MallOrder>(
            addBoughtProductsql,
            new { OrderId = orderId, UserId = userId }
        );
    }

    /// <summary>
    /// 查看單一訂單 ( 訂單編號查詢 )
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <returns>訂單資訊</returns>
    public async Task<MallOrder> GetOrderByOrderNumber(string orderNumber)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Select * From MallOrder
                  Where OrderNumber = @OrderNumber
                 ";

        return await conn.QueryFirstOrDefaultAsync<MallOrder>(addBoughtProductsql, new { OrderNumber = orderNumber });
    }

    /// <summary>
    /// 查看使用者的所有訂單
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>所有訂單資訊</returns>
    public async Task<IEnumerable<OrderResponse>> GetUserAllOrder(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"SELECT m.*,
                   (SELECT TOP 1 productsimg
                    FROM   productimg i
                    WHERE  i.productsid = m.productsid) as ProductsImg
            FROM   mallorder m
            WHERE  m.userid = @UserId ";

        return await conn.QueryAsync<OrderResponse>(addBoughtProductsql, new { UserId = userId });
    }

    /// <summary>
    /// 商品購買
    /// </summary>
    /// <param name="order">購買資訊</param>
    /// <returns>訂單 ID</returns>
    public async Task<int> BuyProducts(MallOrder order)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"INSERT INTO MallOrder
                            (OrderNumber,
                             UserId,
                             ProductsId,
                             BoughtQuantity,
                             UnitPrice,
                             BoughtTime,
                             ShippingAddress,
                             ShippingStatus)

                VALUES      (@OrderNumber,
                             @UserId,
                             @ProductsId,
                             @BoughtQuantity,
                             @UnitPrice,
                             @BoughtTime,
                             @ShippingAddress,
                             @ShippingStatus)
                 Select 
                            Cast(
                            Scope_Identity() as int
                            );";

        return await conn.QuerySingleAsync<int>(addBoughtProductsql, order);
    }

    /// <summary>
    /// 商品付款
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <param name="shippingStatus">運送狀態</param>
    /// <param name="accountPrice">最終金額</param>
    /// <param name="paidType">付款方式</param>
    /// <param name="paidTime">付款時間</param>
    /// <returns>影響列數</returns>
    public async Task<int> PaidProducts(
        string orderNumber,
        int shippingStatus,
        decimal accountPrice,
        string paidType,
        DateTime paidTime
    )
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Update MallOrder
                  Set ShippingStatus = COALESCE(@ShippingStatus, ShippingStatus),
                      AccountPrice = COALESCE(@AccountPrice, AccountPrice),
                      PaidType = COALESCE(@PaidType, PaidType),
                      PaidTime = COALESCE(@PaidTime, PaidTime)
                  Where OrderNumber = @OrderNumber";

        return await conn.ExecuteAsync(
            addBoughtProductsql,
            new
            {
                OrderNumber = orderNumber,
                ShippingStatus = shippingStatus,
                AccountPrice = accountPrice,
                PaidType = paidType,
                PaidTime = paidTime,
            }
        );
    }
}
