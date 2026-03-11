namespace Lab.API.TODO.Common.Requests
{
    public class UpdateRequest
    {
        // 更新的請求格式
        /// <summary>
        /// ID
        /// </summary>
        [Display(Name = "ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        [Required(ErrorMessage = "{0} 必輸")]
        [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1}")]
        public string Name { get; set; }

        /// <summary>
        /// 信箱
        /// </summary>
        [Display(Name = "信箱")]
        [Required(ErrorMessage = "{0} 必輸")]
        [EmailAddress(ErrorMessage = "信箱格式不正確")]
        public string Email { get; set; }
    }
}
