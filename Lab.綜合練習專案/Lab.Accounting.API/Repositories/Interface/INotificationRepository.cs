using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;
using MailKit.Search;

namespace Lab.Accounting.API.Repositories;

public interface INotificationRepository
{
    /// <summary>
    /// 新增一個通知紀錄
    /// </summary>
    /// <param name="notification">通知訊息</param>
    /// <returns>通知 ID</returns>
    Task<int> CreateNotification(Notification notification);
}
