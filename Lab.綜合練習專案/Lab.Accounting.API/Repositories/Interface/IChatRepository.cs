using Lab.Accounting.API.Common.Requests.Coupon;

namespace Lab.Accounting.API.Repositories.Interface;

public interface IChatRepository
{
    /// <summary>
    /// 取得聊天室使用者列表
    /// </summary>
    /// <param name="userId">用戶 ID</param>
    /// <returns>訊息紀錄</returns>
    Task<IEnumerable<ChatUserResponse>> GetChatUserList(int userId);

    /// <summary>
    /// 取得訊息歷史紀錄
    /// </summary>
    /// <param name="senderId">寄送人 ID</param>
    /// <param name="receiverId">接收人 ID</param>
    /// <returns>訊息紀錄</returns>
    Task<IEnumerable<ChatMessage>> GetMessageHistory(int senderId, int receiverId);

    /// <summary>
    /// 儲存訊息
    /// </summary>
    /// <param name="chatMessage">訊息內容</param>
    /// <returns>訊息 ID</returns>
    Task<int> SaveMessage(ChatMessage chatMessage);

    /// <summary>
    /// 改變已讀狀態
    /// </summary>
    /// <param name="senderId">寄送人 ID</param>
    /// <param name="receiverId">接收人 ID</param>
    Task UpdateReadStatus(int senderId, int receiverId);
}
