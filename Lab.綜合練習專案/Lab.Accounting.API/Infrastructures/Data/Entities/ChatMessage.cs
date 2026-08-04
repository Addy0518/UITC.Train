namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class ChatMessage
    {
        /// <summary>
        /// 訊息 ID
        /// </summary>
        public int ChatMessageId { get; set; }

        /// <summary>
        /// 發送者 ID
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// 接收者 ID
        /// </summary>
        public int ReceiverId { get; set; }

        /// <summary>
        /// 訊息內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 發送時間
        /// </summary>
        public DateTime SendTime { get; set; }

        /// <summary>
        /// 是否已讀
        /// </summary>
        public bool IsRead { get; set; }
    }
}
