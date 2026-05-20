using System.Diagnostics.Eventing.Reader;

namespace Lab.Accounting.API.Repositories;

public class ProductsOrderRepository(DBConnecting connecting) : IProductsOrderRepository
{
    /// <summary>
    /// 買家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="userId">買家 ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<OrderResponse> GetUserOneOrder(int orderId, int userId)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"SELECT m.*,p.IsDelete,
                   (SELECT TOP 1 productsimg
                    FROM   productimg i
                    WHERE  i.productsid = m.productsid) as ProductsImg
            FROM   mallorder m
            Left Join   mallproducts p on m.productsid=p.productsid
            Where OrderId = @OrderId
            And m.UserId = @UserId";

        return await conn.QueryFirstOrDefaultAsync<OrderResponse>(
            addBoughtProductsql,
            new { OrderId = orderId, UserId = userId }
        );
    }

    /// <summary>
    /// 賣家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<OrderResponse> GetSellerOneOrder(int orderId, int sellerId)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"SELECT m.*,p.IsDelete,
                   (SELECT TOP 1 productsimg
                    FROM   productimg i
                    WHERE  i.productsid = m.productsid) as ProductsImg
            FROM   mallorder m
            Left Join   mallproducts p on m.productsid=p.productsid
            Where OrderId = @OrderId
            And m.SellerUserId = @SellerId";

        return await conn.QueryFirstOrDefaultAsync<OrderResponse>(
            addBoughtProductsql,
            new { OrderId = orderId, SellerId = sellerId }
        );
    }

    /// <summary>
    /// 查看買家所有訂單 ( 訂單編號查詢 )
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <returns>多筆訂單資訊</returns>
    public async Task<IEnumerable<MallOrder>> GetOrderByOrderNumber(string orderNumber)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Select * From MallOrder
                  Where OrderNumber = @OrderNumber
                 ";

        return await conn.QueryAsync<MallOrder>(addBoughtProductsql, new { OrderNumber = orderNumber });
    }

    /// <summary>
    /// 買家查看所有訂單
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>所有訂單資訊</returns>
    public async Task<IEnumerable<OrderResponse>> GetUserOrder(int userId)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"SELECT m.*,p.IsDelete,
                   (SELECT TOP 1 productsimg
                    FROM   productimg i
                    WHERE  i.productsid = m.productsid) as ProductsImg
            FROM   mallorder m
            Left Join   mallproducts p on m.productsid=p.productsid
            WHERE  m.userid = @UserId ";

        return await conn.QueryAsync<OrderResponse>(addBoughtProductsql, new { UserId = userId });
    }

    /// <summary>
    /// 賣家查看所有訂單
    /// </summary>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>所有訂單資訊</returns>
    public async Task<IEnumerable<OrderResponse>> GetSellerOrder(int sellerId)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"SELECT m.*,m.UserId,p.IsDelete
                   , (SELECT TOP 1 productsimg
                    FROM   productimg i
                    WHERE  i.productsid = m.productsid) as ProductsImg
            FROM   mallorder m
            Left Join   mallproducts p on m.productsid=p.productsid
            WHERE  m.SellerUserId = @UserId";

        return await conn.QueryAsync<OrderResponse>(addBoughtProductsql, new { UserId = sellerId });
    }

    /// <summary>
    /// 改變運輸狀態
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>影響行數</returns>
    public async Task<int> UpdateShippingStatus(int orderId, ShippingStatusEnum shippingStatus)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Update MallOrder Set ShippingStatus = COALESCE(@ShippingStatus, ShippingStatus)
      
            WHERE  OrderId = @OrderId ";

        return await conn.ExecuteAsync(
            addBoughtProductsql,
            new { OrderId = orderId, ShippingStatus = (int)shippingStatus }
        );
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
                             SellerUserId,
                             UserId,
                             ProductsId,
                             ProductsName,
                             ProductCategoryId,
                             BoughtQuantity,
                             UnitPrice,
                             AccountPrice,
                             BoughtTime,
                             ShippingAddress,
                             ShippingStatus)

                VALUES      (@OrderNumber,
                             @SellerUserId,
                             @UserId,
                             @ProductsId,
                             @ProductsName,
                             @ProductCategoryId,
                             @BoughtQuantity,
                             @UnitPrice,
                             @AccountPrice,
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
    /// <param name="paidType">付款方式</param>
    /// <param name="paidTime">付款時間</param>
    /// <returns>影響列數</returns>
    public async Task<int> PaidProducts(string orderNumber, int shippingStatus, string paidType, DateTime paidTime)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Update MallOrder
                  Set ShippingStatus = COALESCE(@ShippingStatus, ShippingStatus),
                      PaidType = COALESCE(@PaidType, PaidType),
                      PaidTime = COALESCE(@PaidTime, PaidTime)
                  Where OrderNumber = @OrderNumber";

        return await conn.ExecuteAsync(
            addBoughtProductsql,
            new
            {
                OrderNumber = orderNumber,
                ShippingStatus = shippingStatus,
                PaidType = paidType,
                PaidTime = paidTime,
            }
        );
    }

    /// <summary>
    /// 商品重新付款
    /// </summary>
    /// <param name="orderIds">所有訂單 ID</param>
    /// <param name="newOrderNumber">新訂單編號</param>
    /// <returns>訂單 ID</returns>
    public async Task<int> RetryPaidProducts(List<int> orderIds, string newOrderNumber)
    {
        using var conn = connecting.CreateConnecting();

        // 更新所有 ID 的訂單編號
        var addBoughtProductsql =
            @"Update MallOrder
                  Set OrderNumber =@NewOrderNumber
                  Where OrderId in @OrderIds";

        return await conn.ExecuteAsync(
            addBoughtProductsql,
            new { NewOrderNumber = newOrderNumber, OrderIds = orderIds }
        );
    }
}
