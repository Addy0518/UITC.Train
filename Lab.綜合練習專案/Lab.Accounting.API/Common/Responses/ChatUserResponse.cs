namespace Lab.Accounting.API.Common.Responses
{
    public class ChatUserResponse
    {
        /// <summary>
        /// 聊天室用戶 ID
        /// </summary>
        public int ChatPartnerId { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 使用者大頭照
        /// </summary>
        public string? UserHeadshot { get; set; }

        /// <summary>
        /// 使用者性別
        /// </summary>
        public GenderEnum UserGender { get; set; }
    }
}
