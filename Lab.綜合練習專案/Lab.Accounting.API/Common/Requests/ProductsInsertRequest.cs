namespace Lab.Accounting.API.Common.Requests
{
    public class ProductsInsertRequest
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int UserId { get; set; }

        /// <summary>
        /// 商品類別名稱
        /// </summary>
        [Display(Name = "商品類別名稱")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string ProductCategoryName { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        [Display(Name = "商品名稱")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string ProductsName { get; set; }

        /// <summary>
        /// 商品價格
        /// </summary>
        [Display(Name = "商品價格")]
        [Required(ErrorMessage = "{0} 必輸")]
        public decimal ProductsPrice { get; set; }
    }
}
