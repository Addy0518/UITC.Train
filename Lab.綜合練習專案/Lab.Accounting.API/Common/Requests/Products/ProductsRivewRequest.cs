namespace Lab.Accounting.API.Common.Requests.Products
{
    public class ProductsRivewRequest
    {
        /// <summary>
        /// 審核紀錄 ID
        /// </summary>
        [Display(Name = "審核紀錄 ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int ProductsReviewId { get; set; }

        /// <summary>
        /// 負責審核的管理員 ID (待審核時為 null)
        /// </summary>
        [Display(Name = "負責審核的管理員 ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int AdminId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        [Display(Name = "商品 ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int ProductsId { get; set; }

        /// <summary>
        /// 審核狀態 (例如：0=待審核, 1=審核通過, 2=審核失敗)
        /// </summary>
        [Display(Name = "審核狀態")]
        [Required(ErrorMessage = "{0} 必輸")]
        public ReviewStatusEnum ReviewStatus { get; set; }

        /// <summary>
        /// 審核未通過/駁回的原因
        /// </summary>
        [Display(Name = "審核未通過/駁回的原因")]
        public string? NotPassReson { get; set; }
    }
}
