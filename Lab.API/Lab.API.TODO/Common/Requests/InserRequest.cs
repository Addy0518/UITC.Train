namespace Lab.API.TODO.Common.Requests
{
    public class InsertRequest
    {
        // 新增的請求格式
        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "{0} 必輸")]
        [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1}")]
        public string Name { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [Display(Name = "角色")]
        public string Role { get; set; } = "User";

        /// <summary>
        /// 信箱
        /// </summary>
        [Display(Name = "信箱")]
        [Required(ErrorMessage = "{0} 必輸")]
        [EmailAddress(ErrorMessage = "信箱格式不正確")]
        public string Email { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Display(Name = "密碼")]
        [Required(ErrorMessage = "{0} 必輸")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "密碼長度必須在 8 到 20 字元之間")]
        public string Password { get; set; }
    }
}
