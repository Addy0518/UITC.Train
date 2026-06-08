namespace Lab.Accounting.API.Common.Requests.Category
{
    public class CategoryInsertRequest
    {
        /// <summary>
        /// 父類別 ID ( 可為 null，表示該類別為頂層類別 )
        /// </summary>
        [Display(Name = "父類別 ID")]
        public int? ProductParentId { get; set; } = null;

        /// <summary>
        /// 商品類別名稱
        /// </summary>
        [Display(Name = "商品類別名稱")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public string ProductCategoryName { get; set; }

        /// <summary>
        /// 類別圖片檔案
        /// </summary>
        [Display(Name = "類別圖片檔案")]
        public IFormFile? ProductCategoryImgFile { get; set; }
    }
}
