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
        [Range(0, int.MaxValue, ErrorMessage = "{0} 不能小於 {1}")]
        public int pageIndex { get; set; } = 0;

        /// <summary>
        /// 每頁顯示數量
        /// </summary>
        [Display(Name = "每頁顯示數量")]
        [Range(1, 100, ErrorMessage = "{0} 必須介於 {1} 到 {2} 之間")]
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
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
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

        /// 排序條件
        /// ==================================================
        /// <summary>
        /// 分類排序
        /// </summary>
        [Display(Name = "分類排序")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? sortBy { get; set; }

        /// <summary>
        /// 排序方向
        /// </summary>
        [Display(Name = "排序方向")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? sortOrder { get; set; }
    }
}
