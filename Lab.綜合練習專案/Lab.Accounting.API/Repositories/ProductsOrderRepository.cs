using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using Lab.Accounting.API.Common.Requests.Order;
using NPOI.SS.UserModel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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
            FROM   [Order] m
            Left Join   product p on m.productsid=p.productsid
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
            @"SELECT m.*,p.IsDelete,u.UserName as UserName,
                   (SELECT TOP 1 productsimg
                    FROM   productimg i
                    WHERE  i.productsid = m.productsid) as ProductsImg
            FROM   [Order] m
            Left Join   product p on m.productsid=p.productsid
            Left join   [User] u on m.UserId=u.UserId
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
    public async Task<IEnumerable<Order>> GetOrderByOrderNumber(string orderNumber)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Select * From [Order]
                  Where OrderNumber = @OrderNumber
                 ";

        return await conn.QueryAsync<Order>(addBoughtProductsql, new { OrderNumber = orderNumber });
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
            FROM   [Order] m
            Left Join   product p on m.productsid=p.productsid
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
            FROM   [Order] m
            Left Join   product p on m.productsid=p.productsid
            WHERE  m.SellerUserId = @UserId";

        return await conn.QueryAsync<OrderResponse>(addBoughtProductsql, new { UserId = sellerId });
    }

    /// <summary>
    /// 查看所有訂單
    /// </summary>
    /// <param name="request">訂單搜尋請求</param>
    /// <returns>所有訂單資訊</returns>
    public async Task<IEnumerable<OrderResponse>> GetAllOrder(OrderSearchRequest request)
    {
        using var conn = connecting.CreateConnecting();
        int offset = request.pageIndex * request.pageSize;
        var sql =
            @"SELECT    m.*,
                      s.UserName as SellerName,
                      u.UserName as UserName,
                      p.IsDelete,
                      Count(*) over() as TotalCount
            FROM      [Order] m
            LEFT JOIN Product p ON m.ProductsId = p.ProductsId
            LEFT JOIN [User] s       ON m.SellerUserId = s.UserId  
            LEFT JOIN [User] u       ON m.UserId = u.UserId  

            WHERE
            (
                @keyWords IS NULL
                OR (@searchType = 'SellerName'  AND s.UserName    LIKE '%' + @keyWords + '%')
                OR (@searchType = 'UserName'    AND u.UserName    LIKE '%' + @keyWords + '%')
                OR (@searchType = 'ProductsName' AND m.ProductsName LIKE '%' + @keyWords + '%')
            )
            AND (@ShippingStatus IS NULL OR m.ShippingStatus = @ShippingStatus)

            ORDER BY
                CASE WHEN @sortBy = 'AccountAmount' AND @sortOrder = 'asc'  THEN m.AccountAmount END ASC,
                CASE WHEN @sortBy = 'AccountAmount' AND @sortOrder = 'desc' THEN m.AccountAmount END DESC,
                CASE WHEN @sortBy = 'BoughtTime'   AND @sortOrder = 'asc'  THEN m.BoughtTime   END ASC,
                CASE WHEN @sortBy = 'BoughtTime'   AND @sortOrder = 'desc' THEN m.BoughtTime   END DESC,
                m.OrderId

            OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";

        return await conn.QueryAsync<OrderResponse>(
            sql,
            new
            {
                offset = offset,
                pageSize = request.pageSize,
                keyWords = request.keyWords,
                searchType = request.searchType,
                sortBy = request.sortBy,
                sortOrder = request.sortOrder,
                ShippingStatus = request.ShippingStatus,
            }
        );
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
            @"Update [Order] Set ShippingStatus = COALESCE(@ShippingStatus, ShippingStatus)
      
            WHERE  OrderId = @OrderId ";

        return await conn.ExecuteAsync(
            addBoughtProductsql,
            new { OrderId = orderId, ShippingStatus = (int)shippingStatus }
        );
    }

    /// <summary>
    /// 依物流單 ID，批次更新其底下所有訂單的運送狀態
    /// </summary>
    /// <param name="logisticsId">物流單 ID</param>
    /// <param name="shippingStatus">運送狀態</param>
    /// <returns>影響行數</returns>
    public async Task<int> UpdateShippingStatusByLogisticsId(int logisticsId, ShippingStatusEnum shippingStatus)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
        UPDATE [Order]
        SET ShippingStatus = @ShippingStatus
        WHERE LogisticsId = @LogisticsId";

        return await conn.ExecuteAsync(sql, new { LogisticsId = logisticsId, ShippingStatus = (int)shippingStatus });
    }

    /// <summary>
    /// 新增物流訂單編號
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <param name="logisticsId">物流訂單 ID</param>
    /// <returns>影響行數</returns>
    public async Task<int> UpdateLogisticsId(int orderId, int logisticsId)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"Update [Order] Set LogisticsId = @LogisticsId
      
            WHERE  OrderId = @OrderId ";

        return await conn.ExecuteAsync(addBoughtProductsql, new { OrderId = orderId, LogisticsId = logisticsId });
    }

    /// <summary>
    /// 商品購買
    /// </summary>
    /// <param name="order">購買資訊</param>
    /// <returns>訂單 ID</returns>
    public async Task<int> BuyProducts(Order order)
    {
        using var conn = connecting.CreateConnecting();

        var addBoughtProductsql =
            @"INSERT INTO [Order]
                            (OrderNumber,
                             SellerUserId,
                             UserId,
                             ProductsId,
                             ProductsName,
                             ProductCategoryId,
                             BoughtQuantity,
                             UnitPrice,
                             OrginalAmount,
                             PlatformDiscount,
                             AccountAmount,
                             BoughtTime,
                             ShippingStatus)

                VALUES      (@OrderNumber,
                             @SellerUserId,
                             @UserId,
                             @ProductsId,
                             @ProductsName,
                             @ProductCategoryId,
                             @BoughtQuantity,
                             @UnitPrice,
                             @OrginalAmount,
                             @PlatformDiscount,
                             @AccountAmount,
                             @BoughtTime,
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
            @"Update [Order]
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
            @"Update [Order]
              Set OrderNumber =@NewOrderNumber
              Where OrderId in @OrderIds";

        return await conn.ExecuteAsync(
            addBoughtProductsql,
            new { NewOrderNumber = newOrderNumber, OrderIds = orderIds }
        );
    }
}
