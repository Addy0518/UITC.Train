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
    /// 依訂單 ID 查詢物流資訊（買家查訂單進度用）
    /// </summary>
    /// <param name="orderId">物流訂單資訊</param>
    /// <returns>物流訂單資訊</returns>
    Task<OrderLogistics?> GetByOrderId(int orderId);

    /// <summary>
    /// 依綠界物流追蹤編號查詢（ServerReplyURL 收到通知時用來對應是哪一筆）
    /// </summary>
    /// <param name="logisticsTrackingNo">物流訂單編號</param>
    /// <returns>物流訂單資訊</returns>
    Task<OrderLogistics?> GetByTrackingNo(string logisticsTrackingNo);

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
    ///<param name="timeStamp">時間戳記</param>
    /// <returns>物流訂單資訊</returns>
    Task UpdateStatus(int logisticsId, LogisticsStatusEnum status, DateTime? timeStamp = null);

    /// <summary>
    /// 取消物流訂單
    /// </summary>
    /// <param name="logisticsId">物流訂單 ID</param>
    /// <returns></returns>
    Task CancelLogistics(int logisticsId);
}
