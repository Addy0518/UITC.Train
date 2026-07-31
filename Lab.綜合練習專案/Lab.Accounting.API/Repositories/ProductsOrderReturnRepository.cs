using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using Lab.Accounting.API.Common.Requests.Order;
using NPOI.SS.UserModel;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Lab.Accounting.API.Repositories;

public class ProductsOrderReturnRepository(DBConnecting connecting) : IProductsOrderReturnRepository
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
            @"SELECT m.*,m.LogisticsId, 
            o.LogisticsType, o.LogisticsSubType, o.StoreCode, o.StoreName, o.StoreAddress, 
            o.ReceiverName, o.ReceiverPhone, o.ReceiverAddress, o.LogisticsStatus,
            o.LogisticsRtnCode, o.LogisticsRtnMessage, o.CreatedAt, 
            o.ShippedAt, o.DeliveredAt, o.PickedUpAt,p.IsDelete,
                   (SELECT TOP 1 productsimg
                    FROM   productimg i
                    WHERE  i.productsid = m.productsid) as ProductsImg
            FROM   [Order] m
            Left Join   product p on m.productsid=p.productsid
            Left Join   OrderLogistics o on m.LogisticsId=o.LogisticsId
            Where OrderId = @OrderId
            And m.UserId = @UserId";

        return await conn.QueryFirstOrDefaultAsync<OrderResponse>(
            addBoughtProductsql,
            new { OrderId = orderId, UserId = userId }
        );
    }
}
