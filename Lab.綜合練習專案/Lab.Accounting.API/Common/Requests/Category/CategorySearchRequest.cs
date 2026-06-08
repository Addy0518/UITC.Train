namespace Lab.Accounting.API.Common.Requests.Category
{
    public class CategorySearchRequest
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
        /// 關鍵字搜尋
        /// </summary>
        [Display(Name = "關鍵字搜尋")]
        public string? keyWords { get; set; }

        /// <summary>
        /// 父類別 ID
        /// </summary>
        [Display(Name = "父類別 ID")]
        public int? parentId { get; set; }
    }
}
