using Lab.Accounting.API.Common.Requests.Logistics;

namespace Lab.Accounting.API.Services;

public interface ILogisticsService
{
    /// <summary>
    /// 產生綠界超商門市地圖網址
    /// </summary>
    /// <param name="request">物流訂單資訊</param>
    /// <returns>地圖網址</returns>
    ApiResponse<Dictionary<string, string>> GetCvsMapUrl(GetCvsMapRequest request);

    /// <summary>
    /// 接收綠界物流狀態通知，更新對應物流單的狀態
    /// </summary>
    /// <param name="request">綠界回傳的物流狀態資料</param>
    /// <returns>是否處理成功</returns>
    Task<bool> HandleLogisticsStatusNotify(LogisticsStatusCallbackRequest request);

    /// <summary>
    /// 呼叫綠界建立物流訂單 ( 超商 )
    /// </summary>
    /// <param name="request">物流訂單資訊</param>
    /// <returns>綠界回傳資料</returns>
    Task<ApiResponse<Dictionary<string, string>>> CreateCVSLogisticsOrder(LogisticsOrderInfoRequest request);

    /// <summary>
    /// 呼叫綠界建立物流訂單 ( 宅配 )
    /// </summary>
    /// <param name="request">物流訂單資訊</param>
    /// <returns>綠界回傳資料</returns>
    Task<ApiResponse<Dictionary<string, string>>> CreateHomeLogisticsOrder(LogisticsOrderInfoRequest request);

    /// <summary>
    /// 儲存物流暫存訂單資料 ( 超商 )
    /// </summary>
    /// <param name="request">綠界回傳門市資料</param>
    /// <returns>是否成功</returns>
    Task<ApiResponse<string>> SaveCvsLogisticsTemp(CvsStoreCallbackRequest request);

    /// <summary>
    /// 儲存物流暫存訂單收件人 ( 超商 )
    /// </summary>
    /// <param name="request">收件人資訊</param>
    /// <returns>是否成功</returns>
    Task<ApiResponse<string>> SaveCvsReceiver(CvsReceiverInsertRequest request);

    /// 儲存物流暫存訂單資料 ( 宅配 )
    /// </summary>
    /// <param name="request">物流暫存表單資料</param>
    /// <returns>是否成功</returns>
    Task<ApiResponse<string>> SaveHomeLogisticsTemp(LogisticsTempInsertRequest request);

    /// <summary>
    /// 查看物流暫存訂單資料
    /// </summary>
    /// <param name="sessionKey">SessionKey ( 對應金流的 MerchantTradeNo )</param>
    /// <returns>物流暫存訂單資料</returns>
    Task<ApiResponse<OrderLogisticsTemp>> GetLogisticsTemp(string sessionKey);

    Task<string> GetCheckMacValueForTest(Dictionary<string, string> parameters);
}
