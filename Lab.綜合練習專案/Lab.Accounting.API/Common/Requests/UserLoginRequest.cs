namespace Lab.Accounting.API.Common.Requests
{
    public class UserLoginRequest
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        [Display(Name = "使用者帳號")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string UserAccount { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        [Display(Name = "使用者密碼")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string UserPassword { get; set; }
    }
}
