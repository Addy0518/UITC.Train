namespace Lab.Accounting.API.Common.Requests
{
    public class ProductsRateRequest
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int UserId { get; set; }

        /// <summary>
        /// 訂單 ID
        /// </summary>
        [Display(Name = "訂單 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int OrderId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        [Display(Name = "商品 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int ProductsId { get; set; }

        /// <summary>
        /// 評分
        /// </summary>
        [Display(Name = "評分")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [Range(1, 5, ErrorMessage = "{0} 必須在 1 到 5 之間!")]
        public double Rating { get; set; }

        /// <summary>
        /// 評論
        /// </summary>
        [Display(Name = "評論")]
        public string? Comment { get; set; }
    }
}
