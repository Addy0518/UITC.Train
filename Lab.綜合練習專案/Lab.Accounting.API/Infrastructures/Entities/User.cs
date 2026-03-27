namespace Lab.Accounting.API.Infrastructures.Entities
{
    public class User
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 使用者帳號
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        public string UserPassword { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 使用者電話
        /// </summary>
        public string UserPhone { get; set; }
    }
}
