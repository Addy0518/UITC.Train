using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;
using MailKit.Search;

namespace Lab.Accounting.API.Repositories;

public interface ILogisticsTempRepository
{
    /// <summary>
    /// 儲存物流暫存訂單資料 ( 先把地址記下來 , 等待付款完成後再建立正式物流訂單 )
    /// </summary>
    /// <param name="logisticsTemp">物流暫存訂單資訊</param>
    /// <returns></returns>
    Task CreateCVSLogisticsTemp(OrderLogisticsTemp logisticsTemp);

    /// <summary>
    /// 儲存物流暫存訂單資料 ( 宅配 )
    /// </summary>
    /// <param name="logisticsTemp">物流暫存訂單資訊</param>
    /// <returns></returns>
    Task CreateHomeLogisticsTemp(OrderLogisticsTemp logisticsTemp);

    /// <summary>
    /// 儲存物流暫存訂單收件人 ( 超商 )
    /// </summary>
    /// <param name="logisticsTemp">物流暫存訂單資訊</param>
    /// <returns></returns>
    Task UpdateCVSLogisticsTemp(OrderLogisticsTemp logisticsTemp);

    /// <summary>
    /// 查看物流暫存訂單資料
    /// </summary>
    /// <param name="sessionKey">SessionKey ( 對應金流的 MerchantTradeNo )</param>
    /// <returns>物流暫存訂單資料</returns>
    Task<OrderLogisticsTemp> GetLogisticsTemp(string sessionKey);

    /// <summary>
    /// 刪除暫存物流訂單資料
    /// </summary>
    /// <param name="sessionKey">SessionKey ( 對應金流的 MerchantTradeNo )</param>
    /// <returns>物流訂單資訊</returns>
    Task<int> DeleteBySessionKey(string sessionKey);

    /// <summary>
    /// 刪除過期的暫存物流訂單資料
    /// </summary>
    /// <returns></returns>
    Task DeleteExpiredLogisticsTemp();
}
