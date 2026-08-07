using Lab.Accounting.API.Common.Requests.Store;

namespace Lab.Accounting.API.Repositories
{
    public class ChatRepository(DBConnecting connecting) : IChatRepository
    {
        /// <summary>
        /// 取得聊天室使用者列表
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>訊息紀錄</returns>
        public async Task<IEnumerable<ChatUserResponse>> GetChatUserList(int userId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Select Distinct 
                            Case
                               When c.SenderId = @UserId Then c.ReceiverId
                               Else c.SenderId  
                            end as ChatPartnerId,
                            u.UserName,
                            u.UserHeadshot,
                            u.UserGender
                        From [dbo].chatmessage as c
                        Left Join [User] as u on u.UserId =  
                            Case
                               When c.SenderId = @UserId Then c.ReceiverId
                               Else c.SenderId
                            end
                        Where c.SenderId = @UserId or c.ReceiverId = @UserId";

            return await conn.QueryAsync<ChatUserResponse>(sql, new { UserId = userId });
        }

        /// <summary>
        /// 取得訊息歷史紀錄
        /// </summary>
        /// <param name="senderId">寄送人 ID</param>
        /// <param name="receiverId">接收人 ID</param>
        /// <returns>訊息紀錄</returns>
        public async Task<IEnumerable<ChatMessage>> GetMessageHistory(int senderId, int receiverId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"SELECT *
                        FROM   [dbo].chatmessage
                        WHERE  ( senderid = @SenderId
                                 AND receiverid = @ReceiverId )
                                OR ( senderid = @ReceiverId
                                     AND receiverid = @SenderId )
                        ORDER  BY sendtime asc ";

            return await conn.QueryAsync<ChatMessage>(sql, new { SenderId = senderId, ReceiverId = receiverId });
        }

        /// <summary>
        /// 儲存訊息
        /// </summary>
        /// <param name="chatMessage">訊息內容</param>
        /// <returns>訊息 ID</returns>
        public async Task<int> SaveMessage(ChatMessage chatMessage)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"INSERT INTO [dbo].ChatMessage
                            (SenderId,
                             ReceiverId,
                             Content,
                             SendTime,
                             IsRead)
                  VALUES    (@SenderId,
                             @ReceiverId,
                             @Content,
                             @SendTime,
                             @IsRead)

                  Select 
                  Cast(
                    Scope_Identity() as int
                  );";

            return await conn.QuerySingleAsync<int>(sql, chatMessage);
        }

        /// <summary>
        /// 改變已讀狀態
        /// </summary>
        /// <param name="senderId">寄送人 ID</param>
        /// <param name="receiverId">接收人 ID</param>
        public async Task UpdateReadStatus(int senderId, int receiverId)
        {
            using var conn = connecting.CreateConnecting();

            var sql =
                @"Update [dbo].ChatMessage Set IsRead=1 
                WHERE SenderId = @SenderId 
                AND ReceiverId = @ReceiverId 
                AND IsRead = 0";

            await conn.ExecuteAsync(sql, new { SenderId = senderId, ReceiverId = receiverId });
        }
    }
}
