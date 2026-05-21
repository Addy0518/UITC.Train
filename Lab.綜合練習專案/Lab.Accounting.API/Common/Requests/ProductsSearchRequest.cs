namespace Lab.Accounting.API.Common.Requests
{
    public class ProductsSearchRequest
    {
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
        /// 是否為刪除狀態
        /// </summary>
        [Display(Name = "是否為刪除狀態")]
        public IsDeleteStatusEnum? isDelete { get; set; } = IsDeleteStatusEnum.Normal;
    }
}
