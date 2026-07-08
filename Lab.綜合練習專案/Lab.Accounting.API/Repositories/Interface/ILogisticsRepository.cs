using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Repositories;

public interface ILogisticsRepository
{
    /// <summary>
    /// 建立物流訂單成功後，新增一筆物流紀錄
    /// </summary>
    /// <param name="logistics">物流訂單資訊</param>
    /// <returns>物流訂單 ID</returns>
    Task<int> CreateLogistics(OrderLogistics logistics);

    /// <summary>
    /// 查看單筆訂單所有物流單
    /// </summary>
    /// <param name="orderNumber">訂單編號</param>
    /// <returns>物流訂單資訊</returns>
    Task<IEnumerable<OrderLogistics>> GetByOrderNumber(string orderNumber);

    /// <summary>
    /// 依綠界物流追蹤編號查詢（ServerReplyURL 收到通知時用來對應是哪一筆）
    /// </summary>
    /// <param name="logisticsTrackingNo">物流訂單編號</param>
    /// <returns>物流訂單資訊</returns>
    Task<OrderLogistics?> GetByTrackingNo(string logisticsTrackingNo);

    /// <summary>
    ///查看多筆訂單下對應的物流單 ID
    /// </summary>
    /// <param name="orderIds">訂單編號列表</param>
    /// <returns>物流訂單 ID 列表</returns>
    Task<IEnumerable<int>> GetLogisticsIdsByOrderIds(List<int> orderIds);

    /// <summary>
    /// 更新物流追蹤編號（呼叫建立物流 API 成功後，把綠界回傳的 LogisticsTrackingNo 存進來）
    /// </summary>
    /// <param name="logisticsId">物流訂單 ID</param>
    /// <param name="logisticsTrackingNo">物流訂單編號</param>
    /// <returns>物流訂單資訊</returns>
    Task UpdateTrackingNo(int logisticsId, string logisticsTrackingNo);

    /// <summary>
    /// 更新物流狀態（綠界 ServerReplyURL 回呼時用）
    /// </summary>
    ///<param name="logisticsId">物流訂單 ID</param>
    ///<param name="status">物流狀態</param>
    ///<param name="rtnCode">物流狀態碼</param>
    ///<param name="rtnMessage">物流狀態訊息</param>
    ///<param name="timeStamp">時間戳記</param>
    /// <returns>物流訂單資訊</returns>
    Task UpdateStatus(
        int logisticsId,
        LogisticsStatusEnum status,
        string? rtnCode = null,
        string? rtnMessage = null,
        DateTime? timeStamp = null
    );

    /// <summary>
    /// 更新物流單訂單編號
    /// </summary>
    /// <param name="logisticsId">物流訂單 ID</param>
    /// <param name="merchantTradeNo">訂單編號</param>
    /// <returns>物流訂單資訊</returns>
    Task UpdateMerchantTradeNo(int logisticsId, string merchantTradeNo);
}
