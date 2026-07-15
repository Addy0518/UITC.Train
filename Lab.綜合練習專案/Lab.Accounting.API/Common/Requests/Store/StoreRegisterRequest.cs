namespace Lab.Accounting.API.Common.Requests.Store
{
    public class StoreRegisterRequest
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int UserId { get; set; }

        /// <summary>
        /// 賣場名稱
        /// </summary>
        [Display(Name = "賣家名稱")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string StoreName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        [Display(Name = "統一編號")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "統編必須為 8 個數字")]
        public string StoreUnifiedNumber { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        [Display(Name = "公司名稱")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? StoreCompanyName { get; set; }
    }
}
