namespace Lab.Accounting.API.Common.Requests.Notification;

public class NotificationSearchRequest
{
    /// 分頁
    /// ==================================================
    /// <summary>
    /// 頁碼
    /// </summary>
    [Display(Name = "頁碼")]
    [Range(0, int.MaxValue, ErrorMessage = "{0} 不能小於 {1}")]
    public int pageIndex { get; set; } = 0;

    /// <summary>
    /// 每頁顯示數量
    /// </summary>
    [Display(Name = "每頁顯示數量")]
    [Range(1, 100, ErrorMessage = "{0} 必須介於 {1} 到 {2} 之間")]
    public int pageSize { get; set; } = 10;

    /// 搜尋條件
    /// ==================================================
    /// <summary>
    /// 被通知者 ID
    /// </summary>
    [Display(Name = "被通知者 ID")]
    public int? UserId { get; set; }
}
