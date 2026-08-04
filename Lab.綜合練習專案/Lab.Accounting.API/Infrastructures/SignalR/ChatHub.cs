using Microsoft.AspNetCore.SignalR;

namespace Lab.Accounting.API.Infrastructures.SignalR
{
    public class ChatHub : Hub
    {
        // 存 UserId 對應 ConnectionId，靜態所以全域共享
        private static Dictionary<int, string> _onlineUsers = new();

        /// <summary>
        /// 使用者連線時呼叫，把 UserId 跟 ConnectionId 對應起來
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // 從 Query String 拿 UserId
            // 前端連線時要帶：/chatHub?userId=123
            var userIdStr = Context.GetHttpContext()?.Request.Query["userId"];
            if (int.TryParse(userIdStr, out int userId))
            {
                _onlineUsers[userId] = Context.ConnectionId;
            }
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 使用者斷線時清除
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // 找到這個 ConnectionId 對應的 UserId 並移除
            var user = _onlineUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (user.Key != 0)
            {
                _onlineUsers.Remove(user.Key);
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 傳送訊息
        /// </summary>
        public async Task SendMessage(int receiverId, string content)
        {
            // 找到接收者的 ConnectionId
            if (_onlineUsers.TryGetValue(receiverId, out string? connectionId))
            {
                // 推訊息給接收者
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", content);
            }
        }
    }
}
