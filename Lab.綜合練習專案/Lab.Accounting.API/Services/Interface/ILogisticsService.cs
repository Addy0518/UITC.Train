namespace Lab.Accounting.API.Services;

public interface ILogisticsService
{
    /// <summary>
    /// 產生綠界超商門市地圖網址
    /// </summary>
    /// <param name="request">物流訂單資訊</param>
    /// <returns>地圖網址</returns>
    ApiResponse<string> GetCvsMapUrl(GetCvsMapRequest request);

    /// <summary>
    /// 接收綠界回傳的門市資料存進暫存表
    /// </summary>
    /// <param name="request">綠界回傳門市資料</param>
    /// <returns></returns>
    Task HandleCvsStoreCallback(CvsStoreCallbackRequest request);

    /// <summary>
    /// 收件人資料存進暫存表
    /// </summary>
    /// <param name="request">物流暫存表單資料</param>
    /// <returns>操作結果</returns>
    Task<ApiResponse<string>> SaveLogisticsTemp(LogisticsTempInsertRequest request);
}
