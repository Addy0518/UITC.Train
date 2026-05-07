namespace Lab.Accounting.API.Common.Requests
{
    public class UserUpdateRequest
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
        /// 使用者電話
        /// </summary>
        public int? UserPhone { get; set; }

        /// <summary>
        /// 使用者地址
        /// </summary>
        public string? UserAddress { get; set; }

        /// <summary>
        /// 使用者權限
        /// </summary>
        public string? UserRole { get; set; }
    }
}
