using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;
using MailKit.Search;

namespace Lab.Accounting.API.Repositories;

public class LogisticsTempRepository(DBConnecting connecting) : ILogisticsTempRepository
{
    /// <summary>
    /// 儲存物流暫存訂單資料 ( 超商 )
    /// </summary>
    /// <param name="logisticsTemp">物流暫存訂單資訊</param>
    /// <returns></returns>
    public async Task CreateCVSLogisticsTemp(OrderLogisticsTemp logisticsTemp)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
              INSERT INTO OrderLogisticsTemp (
                SessionKey, LogisticsType, LogisticsSubType,
                StoreCode, StoreName, StoreAddress,
                ReceiverName, ReceiverPhone,
                ExpiredAt
            )
            VALUES (
                @SessionKey, @LogisticsType, @LogisticsSubType,
                @StoreCode, @StoreName, @StoreAddress,
                @ReceiverName, @ReceiverPhone,
                @ExpiredAt
            );";
        await conn.ExecuteAsync(sql, logisticsTemp);
    }

    /// <summary>
    /// 儲存物流暫存訂單資料 ( 宅配 )
    /// </summary>
    /// <param name="logisticsTemp">物流暫存訂單資訊</param>
    /// <returns></returns>
    public async Task CreateHomeLogisticsTemp(OrderLogisticsTemp logisticsTemp)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
            INSERT INTO OrderLogisticsTemp (
                SessionKey, LogisticsType, LogisticsSubType,
                ReceiverName, ReceiverPhone,ReceiverAddress,ReceiverZipCode,
                ExpiredAt
            )
            VALUES (
                @SessionKey, @LogisticsType, @LogisticsSubType,
                @ReceiverName, @ReceiverPhone,@ReceiverAddress,@ReceiverZipCode,
                @ExpiredAt
            );";
        await conn.ExecuteAsync(sql, logisticsTemp);
    }

    /// <summary>
    /// 儲存物流暫存訂單收件人 ( 超商 )
    /// </summary>
    /// <param name="logisticsTemp">物流暫存訂單資訊</param>
    /// <returns></returns>
    public async Task UpdateCVSLogisticsTemp(OrderLogisticsTemp logisticsTemp)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"
        UPDATE OrderLogisticsTemp
        SET ReceiverName = @ReceiverName,
            ReceiverPhone = @ReceiverPhone,
            ExpiredAt = @ExpiredAt
        WHERE SessionKey = @SessionKey";
        await conn.ExecuteAsync(sql, logisticsTemp);
    }

    /// <summary>
    /// 查看物流暫存訂單資料
    /// </summary>
    /// <param name="sessionKey">SessionKey</param>
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
    /// <param name="sessionKey">SessionKey</param>
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
