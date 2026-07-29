using Lab.Accounting.API.Common.Requests.Notification;
using Lab.Accounting.API.Common.Requests.Products;
using Lab.Accounting.API.Common.Requests.Store;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;

namespace Lab.Accounting.API.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// 查看所有通知紀錄
        /// </summary>
        /// <param name="request">通知搜尋請求</param>
        /// <returns>所有通知訊息</returns>
        Task<ApiResponse<NotificationResponse>> GetAllNotifications(NotificationSearchRequest request);

        /// <summary>
        /// 查看單一通知紀錄
        /// </summary>
        /// <param name="notificationId">通知 ID</param>
        ///  <param name="userId">使用者 ID</param>
        /// <returns>單一通知訊息</returns>
        Task<ApiResponse<OneNotification>> GetNotification(int notificationId, int userId);

        /// <summary>
        /// 新增一個通知紀錄
        /// </summary>
        /// <param name="userId">通知要給誰看</param>
        /// <param name="type">通知類型</param>
        /// <param name="title">通知標題</param>
        /// <param name="content">通知內容</param>
        /// <param name="relatedId">相關聯的 ID</param>
        Task CreateNotification(
            int userId,
            NotificationTypeEnum type,
            string title,
            string content,
            int? relatedId = null
        );

        /// <summary>
        /// 改變所有通知已讀狀態
        /// </summary>
        /// <param name="userId">用戶 ID </param>
        /// <param name="isRead">是否已讀</param>
        /// <returns>影響列數</returns>
        Task<int> UpdateAllNotificationReadStatus(int userId, bool isRead);
    };
}
