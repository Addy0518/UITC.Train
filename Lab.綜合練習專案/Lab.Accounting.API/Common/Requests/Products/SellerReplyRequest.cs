namespace Lab.Accounting.API.Common.Requests.Products
{
    public class SellerReplyRequest
    {
        /// <summary>
        /// 訂單 ID
        /// </summary>
        [Display(Name = "訂單 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int OrderId { get; set; }

        /// <summary>
        /// 賣家 ID
        /// </summary>
        [Display(Name = "賣家 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int SellerId { get; set; }

        /// <summary>
        /// 賣家回覆內容
        /// </summary>
        [Display(Name = "賣家回覆內容")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(3000, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string Reply { get; set; } = string.Empty;
    }
}
