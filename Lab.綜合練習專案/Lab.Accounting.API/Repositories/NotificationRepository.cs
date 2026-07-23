using Lab.Accounting.API.Common.Requests.Category;
using Lab.Accounting.API.Common.Requests.Coupon;
using MailKit.Search;

namespace Lab.Accounting.API.Repositories;

public class NotificationRepository(DBConnecting connecting)
{
    /// <summary>
    /// 新增一個通知紀錄
    /// </summary>
    /// <param name="notification">通知訊息</param>
    /// <returns>通知 ID</returns>
    public async Task<int> CreateNotification(Notification notification)
    {
        using var conn = connecting.CreateConnecting();

        var sql =
            @"
            INSERT INTO Notifications (
                UserId , NotificationType, Title,[Content],RelatedId,IsRead,CreateTime
            )
            VALUES (
                @UserId ,@NotificationType,@Title,@[Content],@RelatedId,@IsRead,@CreateTime
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        return await conn.ExecuteScalarAsync<int>(sql, notification);
    }
}
