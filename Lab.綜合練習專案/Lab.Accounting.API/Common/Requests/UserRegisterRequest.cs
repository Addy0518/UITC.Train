namespace Lab.Accounting.API.Common.Requests
{
    public class UserRegisterRequest
    {
        /// <summary>
        /// 使用者帳號
        /// </summary>
        [Display(Name = "使用者帳號")]
        [Required(ErrorMessage = "{0} 必輸")]
        [EmailAddress(ErrorMessage = "{0} 必須符合格式")]
        public string UserAccount { get; set; }

        /// <summary>
        /// 使用者密碼
        /// </summary>
        [Display(Name = "使用者密碼")]
        [Required(ErrorMessage = "{0} 必輸")]
        [RegularExpression(
            @"^[A-Z](?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{7}$",
            ErrorMessage = "密碼總共 8 個字 , 只能輸入英文跟數字 , 第一個字要大寫"
        )]
        public string UserPassword { get; set; }

        /// <summary>
        /// 使用者名稱
        /// </summary>
        [Display(Name = "使用者名稱")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string UserName { get; set; }

        /// <summary>
        /// 使用者電話
        /// </summary>
        [Display(Name = "使用者電話")]
        public string? UserPhone { get; set; }
    }
}
