using Lab.Accounting.API.Common.Requests.Notification;
using Lab.Accounting.API.Common.Requests.Products;
using Lab.Accounting.API.Common.Requests.Store;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Org.BouncyCastle.Asn1.X509;

namespace Lab.Accounting.API.Services
{
    public class NotificationService(INotificationRepository notificationRepository) : INotificationService
    {
        /// <summary>
        /// 查看所有通知紀錄
        /// </summary>
        /// <param name="request">通知搜尋請求</param>
        /// <returns>所有通知訊息</returns>
        public async Task<ApiResponse<NotificationResponse>> GetAllNotifications(NotificationSearchRequest request)
        {
            var result = await notificationRepository.GetAllNotifications(request);
            var response = new NotificationResponse
            {
                Notifications = result,
                TotalCount = result.FirstOrDefault()?.TotalCount ?? 0,
            };
            return ApiResponseHelper.Success(response);
        }

        /// <summary>
        /// 查看單一通知紀錄
        /// </summary>
        /// <param name="notificationId">通知 ID</param>
        ///  <param name="userId">使用者 ID</param>
        /// <returns>單一通知訊息</returns>
        public async Task<ApiResponse<OneNotification>> GetNotification(int notificationId, int userId)
        {
            var result = await notificationRepository.GetNotification(notificationId, userId);
            if (result == null)
                return ApiResponseHelper.NotFound<OneNotification>();
            // 改變狀態為已讀
            await notificationRepository.UpdateNotificationReadStatus(notificationId, userId, true);
            return ApiResponseHelper.Success(result);
        }

        /// <summary>
        /// 新增一個通知紀錄
        /// </summary>
        /// <param name="userId">通知要給誰看</param>
        /// <param name="type">通知類型</param>
        /// <param name="title">通知標題</param>
        /// <param name="content">通知內容</param>
        /// <param name="relatedId">相關聯的 ID</param>
        public async Task CreateNotification(
            int userId,
            NotificationTypeEnum type,
            string title,
            string content,
            int? relatedId = null
        )
        {
            await notificationRepository.CreateNotification(userId, type, title, content, relatedId);
        }
    }
}
