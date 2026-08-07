using Microsoft.AspNetCore.SignalR;
using MimeKit;
using SendGrid.Helpers.Mail;

namespace Lab.Accounting.API.Infrastructures.SignalR
{
    /// <summary>
    /// ChatHub 是 SignalR 即時通訊的中繼站，類似 controller
    /// </summary>
    public class ChatHub(IChatRepository chatRepository) : Hub
    {
        /// <summary>
        ///  存 UserId 對應 ConnectionId，靜態所以全域共享
        /// </summary>
        private static Dictionary<int, string> _onlineUsers = new();

        /// <summary>
        /// 用 override 把 Hub 裡的 OnConnectedAsync 改寫成我的方法 , 而 base.OnConnectedAsync() 是呼叫原本的 Hub 的方法 , 用來連線時觸發
        /// 這裡是把連線時的 Query ( /chatHub?userId=123 ) 拿到 UserId 存起來
        /// 比如 UserId =1 的 , 就會是 { 1 : ConnectionId }
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userIdStr = Context.GetHttpContext()?.Request.Query["userId"];
            if (int.TryParse(userIdStr, out int userId))
            {
                _onlineUsers[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 使用者斷線時清除 , base.OnDisconnectedAsync 就是斷線時觸發
        /// 找到這個 ConnectionId 對應的 UserId 並移除
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _onlineUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (user.Key != 0)
            {
                _onlineUsers.Remove(user.Key);
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 傳送訊息 , Client 指定要傳送的對象 , SendAsync("ReceiveMessage", content) 會觸發前端的 ReceiveMessage 方法
        /// </summary>
        public async Task SendMessage(int senderId, int receiverId, string content)
        {
            await chatRepository.SaveMessage(
                new ChatMessage
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    SendTime = DateTime.UtcNow,
                    IsRead = false,
                }
            );

            if (_onlineUsers.TryGetValue(receiverId, out string? connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, content, DateTime.Now);
            }
        }

        public async Task MarkAsRead(int senderId)
        {
            await chatRepository.UpdateReadStatus(senderId, GetCurrentUserId());

            if (_onlineUsers.TryGetValue(senderId, out string? connectionId))
            {
                await Clients.Client(connectionId).SendAsync("MessageRead", GetCurrentUserId());
            }
        }

        /// <summary>
        /// 從 QueryString 拿 UserId 的
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdStr = Context.GetHttpContext()?.Request.Query["userId"];
            return int.TryParse(userIdStr, out int userId) ? userId : 0;
        }
    }
}
