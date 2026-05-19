namespace Lab.Accounting.API.Common.Requests
{
    public class SellerRegisterRequest
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int UserId { get; set; }

        /// <summary>
        /// 賣家名稱
        /// </summary>
        [Display(Name = "賣家名稱")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public string SellerName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        [Display(Name = "統一編號")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int SellerUnifiedNumber { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        [Display(Name = "公司名稱")]
        public string? SellerCompanyName { get; set; }
    }
}
