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
}
