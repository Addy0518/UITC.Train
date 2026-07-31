using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;
using Lab.Accounting.API.Common.Requests.Notification;
using Lab.Accounting.API.Common.Requests.Products;
using MailKit.Search;

namespace Lab.Accounting.API.Repositories;

public class NotificationRepository(DBConnecting connecting) : INotificationRepository
{
    /// <summary>
    /// 查看所有通知紀錄
    /// </summary>
    /// <param name="request">通知搜尋請求</param>
    /// <returns>所有通知訊息</returns>
    public async Task<IEnumerable<OneNotification>> GetAllNotifications(NotificationSearchRequest request)
    {
        using var conn = connecting.CreateConnecting();
        int offset = request.pageIndex * request.pageSize;

        var sql =
            @"Select   *,
                       Count(*) over() as TotalCount 
                  From dbo.Notification 
                  Where UserId = @UserId
                  Order By CreateTime desc
                  Offset @offset Rows Fetch Next @pageSize Rows Only";

        return await conn.QueryAsync<OneNotification>(
            sql,
            new
            {
                offset = offset,
                pageSize = request.pageSize,
                UserId = request.UserId,
            }
        );
    }

    /// <summary>
    /// 查看單一通知紀錄
    /// </summary>
    /// <param name="notificationId">通知 ID</param>
    ///  <param name="userId">使用者 ID</param>
    /// <returns>單一通知訊息</returns>
    public async Task<OneNotification> GetNotification(int notificationId, int userId)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"SELECT   *
                FROM   Notification 
                WHERE  NotificationId = @NotificationId 
                And    UserId = @UserId";

        return await conn.QueryFirstOrDefaultAsync<OneNotification>(
            sql,
            new { NotificationId = notificationId, UserId = userId }
        );
    }

    /// <summary>
    /// 新增一個通知紀錄
    /// </summary>
    /// <param name="userId">通知要給誰看</param>
    /// <param name="type">通知類型</param>
    /// <param name="title">通知標題</param>
    /// <param name="content">通知內容</param>
    /// <param name="relatedId">相關聯的 ID</param>
    /// <returns>通知 ID</returns>
    public async Task<int> CreateNotification(
        int userId,
        NotificationTypeEnum type,
        string title,
        string content,
        int? relatedId = null
    )
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
            INSERT INTO Notification (
                UserId , NotificationType, Title,Content,RelatedId,IsRead,CreateTime
            )
            VALUES (
                @UserId ,@NotificationType,@Title,@Content,@RelatedId,@IsRead,@CreateTime
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await conn.ExecuteScalarAsync<int>(
            sql,
            new
            {
                UserId = userId,
                NotificationType = type,
                Title = title,
                Content = content,
                RelatedId = relatedId,
                IsRead = false,
                CreateTime = DateTime.Now,
            }
        );
    }

    /// <summary>
    /// 新增多筆通知紀錄
    /// </summary>
    /// <param name="notifications">通知訊息清單</param>
    /// <returns>通知 ID</returns>
    public async Task<int> CreateAllNotifications(IEnumerable<Notification> notifications)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
            INSERT INTO Notification (
                UserId , NotificationType, Title,Content,RelatedId,IsRead,CreateTime
            )
            VALUES (
                @UserId ,@NotificationType,@Title,@Content,@RelatedId,@IsRead,@CreateTime
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await conn.ExecuteAsync(sql, notifications);
    }

    /// <summary>
    /// 改變單一通知已讀狀態
    /// </summary>
    /// <param name="notificationId">通知 ID </param>
    /// <param name="userId">用戶 ID </param>
    /// <param name="isRead">是否已讀</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateNotificationReadStatus(int notificationId, int userId, bool isRead)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"UPDATE dbo.Notification
              SET IsRead = @IsRead
              WHERE NotificationId = @NotificationId and UserId=@UserId";

        return await conn.ExecuteAsync(
            sql,
            new
            {
                NotificationId = notificationId,
                UserId = userId,
                IsRead = isRead,
            }
        );
    }

    /// <summary>
    /// 改變所有通知已讀狀態
    /// </summary>
    /// <param name="userId">用戶 ID </param>
    /// <param name="isRead">是否已讀</param>
    /// <returns>影響列數</returns>
    public async Task<int> UpdateAllNotificationReadStatus(int userId, bool isRead)
    {
        using var conn = connecting.CreateConnecting();
        var sql =
            @"UPDATE dbo.Notification
              SET IsRead = @IsRead
              WHERE UserId=@UserId";

        return await conn.ExecuteAsync(sql, new { UserId = userId, IsRead = isRead });
    }
}
