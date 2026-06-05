namespace Lab.Accounting.API.Common.Requests.Store
{
    public class StoreUpdateRequest
    {
        /// <summary>
        /// 賣場 ID
        /// </summary>
        [Display(Name = "賣場 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int StoreId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int UserId { get; set; }

        /// <summary>
        /// 賣場名稱
        /// </summary>
        [Display(Name = "賣場名稱")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 不能超過 {1} 個字!")]
        public string StoreName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        [Display(Name = "統一編號")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "{0} 格式不正確，須為 8 位數字!")]
        public string StoreUnifiedNumber { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        [Display(Name = "公司名稱")]
        [MaxLength(100, ErrorMessage = "{0} 不能超過 {1} 個字!")]
        public string? StoreCompanyName { get; set; }
    }
}
