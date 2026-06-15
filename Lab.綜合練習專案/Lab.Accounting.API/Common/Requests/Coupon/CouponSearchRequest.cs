namespace Lab.Accounting.API.Common.Requests.Coupon
{
    public class CouponSearchRequest
    {
        /// 分頁
        /// ==================================================
        /// <summary>
        /// 頁碼
        /// </summary>
        [Display(Name = "頁碼")]
        public int pageIndex { get; set; } = 0;

        /// <summary>
        /// 每頁顯示數量
        /// </summary>
        [Display(Name = "每頁顯示數量")]
        public int pageSize { get; set; } = 10;

        /// 搜尋條件
        /// ==================================================
        /// <summary>
        /// 創建者 ID
        /// </summary>
        [Display(Name = "創建者 ID")]
        public int? CreaterId { get; set; }

        /// <summary>
        /// 關鍵字搜尋
        /// </summary>
        [Display(Name = "關鍵字搜尋")]
        public string? keyWords { get; set; }

        /// <summary>
        /// 優惠券類型 (1: 百分比折扣, 2: 固定金額折抵...)
        /// </summary>
        [Display(Name = "優惠券類型")]
        public CouponTypeEnum? Type { get; set; }

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        public bool? IsActive { get; set; }
    }
}
