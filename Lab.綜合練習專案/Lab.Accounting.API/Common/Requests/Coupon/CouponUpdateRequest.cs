namespace Lab.Accounting.API.Common.Requests.Coupon
{
    public class CouponUpdateRequest
    {
        /// <summary>
        /// 優惠券 ID
        /// </summary>
        [Display(Name = "優惠券 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int CouponId { get; set; }

        /// <summary>
        /// 創建者 ID
        /// </summary>
        [Display(Name = "創建者 ID")]
        public int? CreaterId { get; set; }

        /// <summary>
        /// 優惠券名稱
        /// </summary>
        [Display(Name = "優惠券名稱")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(300, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string Name { get; set; }

        /// <summary>
        /// 優惠券類型 (1: 百分比折扣, 2: 固定金額折抵...)
        /// </summary>
        [Display(Name = "優惠券類型")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public CouponTypeEnum Type { get; set; }

        /// <summary>
        /// 折扣數值 (打折趴數或折抵金額)
        /// </summary>
        [Display(Name = "折扣數值")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [Range(1, 100, ErrorMessage = "{0} 必須在 {1} 到 {2} 之間!")]
        public decimal Discount { get; set; }

        /// <summary>
        /// 最低消費門檻
        /// </summary>
        [Display(Name = "最低消費門檻")]
        [Range(0, double.MaxValue, ErrorMessage = "{0} 必須大於或等於 {1}!")]
        public decimal? MinimunSpend { get; set; }

        /// <summary>
        /// 折扣開始時間
        /// </summary>
        [Display(Name = "折扣開始時間")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 折扣結束時間
        /// </summary>
        [Display(Name = "折扣結束時間")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; }
    }
}
