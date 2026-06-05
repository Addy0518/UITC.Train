namespace Lab.Accounting.API.Common.Requests.Order
{
    public class OrderSearchRequest
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
        /// 搜尋類別
        /// </summary>
        [Display(Name = "搜尋類別")]
        public string? searchType { get; set; }

        /// <summary>
        /// 關鍵字搜尋
        /// </summary>
        [Display(Name = "關鍵字搜尋")]
        public string? keyWords { get; set; }

        /// <summary>
        /// 運輸狀態
        /// </summary>
        [Display(Name = "運輸狀態")]
        public ShippingStatusEnum? ShippingStatus { get; set; }

        /// 排序條件
        /// ==================================================
        /// <summary>
        /// 分類排序
        /// </summary>
        [Display(Name = "分類排序")]
        public string? sortBy { get; set; }

        /// <summary>
        /// 排序方向
        /// </summary>
        [Display(Name = "排序方向")]
        public string? sortOrder { get; set; }
    }
}
