namespace Lab.Accounting.API.Common.Requests
{
    public class ProductsSearchRequest
    {
        /// 分頁 / ID
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

        /// <summary>
        /// 賣家 ID
        /// </summary>
        [Display(Name = "賣家 ID")]
        public int? sellerId { get; set; }

        /// <summary>
        /// 商品類別 ID
        /// </summary>
        [Display(Name = "商品類別 ID")]
        public int? productCategoryId { get; set; }

        /// 搜尋條件
        /// ==================================================
        /// <summary>
        /// 關鍵字搜尋
        /// </summary>
        [Display(Name = "關鍵字搜尋")]
        public string? keyWords { get; set; }

        /// <summary>
        /// 是否為刪除狀態
        /// </summary>
        [Display(Name = "是否為刪除狀態")]
        public IsDeleteStatusEnum? isDelete { get; set; } = IsDeleteStatusEnum.Normal;

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

        /// <summary>
        /// 最大價格
        /// </summary>
        [Display(Name = "最大價格")]
        public int? maxPrice { get; set; }

        /// <summary>
        /// 最小價格
        /// </summary>
        [Display(Name = "最小價格")]
        public int? minPrice { get; set; }

        /// <summary>
        /// 評價
        /// </summary>
        [Display(Name = "評價")]
        public int? rate { get; set; }
    }
}
