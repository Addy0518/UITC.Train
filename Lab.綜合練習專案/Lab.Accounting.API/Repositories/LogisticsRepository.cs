using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;
using MailKit.Search;

namespace Lab.Accounting.API.Repositories;

public class LogisticsRepository(DBConnecting connecting) : ILogisticsRepository
{
    /// <summary>
    /// 建立物流訂單成功後，新增一筆物流紀錄
    /// </summary>
    /// <param name="logistics">物流訂單資訊</param>
    /// <returns>物流訂單 ID</returns>
    public async Task<int> CreateLogistics(OrderLogistics logistics)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
            INSERT INTO OrderLogistics (
                OrderId, LogisticsType, LogisticsSubType,
                StoreCode, StoreName, StoreAddress,
                ReceiverName, ReceiverPhone, ReceiverAddress,
                MerchantTradeNo, LogisticsTrackingNo,
                LogisticsStatus, CreatedAt
            )
            VALUES (
                @OrderId, @LogisticsType, @LogisticsSubType,
                @StoreCode, @StoreName, @StoreAddress,
                @ReceiverName, @ReceiverPhone, @ReceiverAddress,
                @MerchantTradeNo, NULL,
                @LogisticsStatus, GETDATE()
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
        return await conn.ExecuteScalarAsync<int>(sql, logistics);
    }

    /// <summary>
    /// 依訂單 ID 查詢物流資訊（買家查訂單進度用）
    /// </summary>
    /// <param name="orderId">物流訂單資訊</param>
    /// <returns>物流訂單資訊</returns>
    public async Task<OrderLogistics?> GetByOrderId(int orderId)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"
            SELECT * FROM OrderLogistics
            WHERE OrderId = @OrderId";

        return await conn.QueryFirstOrDefaultAsync<OrderLogistics>(sql, new { OrderId = orderId });
    }

    /// <summary>
    /// 依綠界物流追蹤編號查詢（ServerReplyURL 收到通知時用來對應是哪一筆）
    /// </summary>
    /// <param name="logisticsTrackingNo">物流訂單編號</param>
    /// <returns>物流訂單資訊</returns>
    public async Task<OrderLogistics?> GetByTrackingNo(string logisticsTrackingNo)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"
            SELECT * FROM OrderLogistics
            WHERE LogisticsTrackingNo = @LogisticsTrackingNo";

        return await conn.QueryFirstOrDefaultAsync<OrderLogistics>(
            sql,
            new { LogisticsTrackingNo = logisticsTrackingNo }
        );
    }

    /// <summary>
    /// 更新物流追蹤編號（呼叫建立物流 API 成功後，把綠界回傳的 LogisticsTrackingNo 存進來）
    /// </summary>
    /// <param name="logisticsId">物流訂單 ID</param>
    /// <param name="logisticsTrackingNo">物流訂單編號</param>
    /// <returns>物流訂單資訊</returns>
    public async Task UpdateTrackingNo(int logisticsId, string logisticsTrackingNo)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"
            UPDATE OrderLogistics
            SET LogisticsTrackingNo = @LogisticsTrackingNo
            WHERE LogisticsId = @LogisticsId";

        await conn.ExecuteAsync(sql, new { LogisticsId = logisticsId, LogisticsTrackingNo = logisticsTrackingNo });
    }

    /// <summary>
    /// 更新物流狀態（綠界 ServerReplyURL 回呼時用）
    /// </summary>
    ///<param name="logisticsId">物流訂單 ID</param>
    ///<param name="status">物流狀態</param>
    ///<param name="timeStamp">時間戳記</param>
    /// <returns>物流訂單資訊</returns>
    public async Task UpdateStatus(int logisticsId, LogisticsStatusEnum status, DateTime? timeStamp = null)
    {
        using var conn = connecting.CreateConnecting();

        // 依狀態決定要更新哪個時間戳記欄位
        // Shipped → ShippedAt
        // Delivered → DeliveredAt
        // PickedUp → PickedUpAt
        // 其他狀態不需要更新時間欄位
        var now = timeStamp ?? DateTime.Now;

        var timeColumn = status switch
        {
            LogisticsStatusEnum.Shipped => "ShippedAt = @TimeStamp,",
            LogisticsStatusEnum.Delivered => "DeliveredAt = @TimeStamp,",
            LogisticsStatusEnum.PickedUp => "PickedUpAt = @TimeStamp,",
            _ => "",
        };

        var sql =
            $@"
            UPDATE OrderLogistics
            SET LogisticsStatus = @LogisticsStatus,
                {timeColumn}
                UpdatedAt = GETDATE()
            WHERE LogisticsId = @LogisticsId";

        await conn.ExecuteAsync(
            sql,
            new
            {
                LogisticsId = logisticsId,
                LogisticsStatus = status.ToString(),
                TimeStamp = now,
            }
        );
    }

    /// <summary>
    /// 取消物流訂單
    /// </summary>
    /// <param name="logisticsId">物流訂單 ID</param>
    /// <returns></returns>
    public async Task CancelLogistics(int logisticsId)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"
             UPDATE OrderLogistics
            SET LogisticsStatus = @LogisticsStatus
            WHERE LogisticsId = @LogisticsId";

        await conn.ExecuteAsync(sql, new { LogisticsId = logisticsId });
    }
}
