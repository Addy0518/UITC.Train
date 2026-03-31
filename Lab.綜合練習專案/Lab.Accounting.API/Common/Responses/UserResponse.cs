namespace Lab.Accounting.API.Common.Responses
{
    public class UserResponse
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string? Token { get; set; }
    }
}
