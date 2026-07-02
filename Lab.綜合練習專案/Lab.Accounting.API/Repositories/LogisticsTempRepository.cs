using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;
using MailKit.Search;

namespace Lab.Accounting.API.Repositories;

public class LogisticsTempRepository(DBConnecting connecting) : ILogisticsTempRepository
{
    /// <summary>
    /// 儲存物流暫存訂單資料
    /// </summary>
    /// <param name="logisticsTemp">物流暫存訂單資訊</param>
    /// <returns></returns>
    public async Task CreateLogisticsTemp(OrderLogisticsTemp logisticsTemp)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
              INSERT INTO OrderLogisticsTemp (
                SessionKey, LogisticsType, LogisticsSubType,
                StoreCode, StoreName, StoreAddress,
                ReceiverName, ReceiverPhone, ReceiverAddress,
                ExpiredAt
            )
            VALUES (
                @SessionKey, @LogisticsType, @LogisticsSubType,
                @StoreCode, @StoreName, @StoreAddress,
                @ReceiverName, @ReceiverPhone, @ReceiverAddress,
                @ExpiredAt
            );";
        await conn.ExecuteAsync(sql, logisticsTemp);
    }

    /// <summary>
    /// 查看物流暫存訂單資料
    /// </summary>
    /// <param name="sessionKey">SessionKey ( 對應金流的 MerchantTradeNo )</param>
    /// <returns>物流暫存訂單資料</returns>
    public async Task<OrderLogisticsTemp> GetLogisticsTemp(string sessionKey)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
            SELECT * FROM OrderLogisticsTemp
            WHERE SessionKey = @SessionKey";
        return await conn.QueryFirstOrDefaultAsync<OrderLogisticsTemp>(sql, new { SessionKey = sessionKey });
    }

    /// <summary>
    /// 刪除暫存物流訂單資料
    /// </summary>
    /// <param name="sessionKey">SessionKey ( 對應金流的 MerchantTradeNo )</param>
    /// <returns>物流訂單資訊</returns>
    public async Task<int> DeleteBySessionKey(string sessionKey)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"
            Delete FROM OrderLogisticsTemp
            WHERE SessionKey = @SessionKey";

        return await conn.ExecuteAsync(sql, new { SessionKey = sessionKey });
    }

    /// <summary>
    /// 刪除過期的暫存物流訂單資料
    /// </summary>
    /// <returns></returns>
    public async Task DeleteExpiredLogisticsTemp()
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"
            DELETE FROM OrderLogisticsTemp WHERE ExpiredAt < GETDATE()";

        await conn.ExecuteAsync(sql);
    }
}
