namespace Lab.Accounting.API.Common.Requests
{
    public class CategoryInsertRequest
    {
        /// <summary>
        /// 父類別 ID ( 可為 null，表示該類別為頂層類別 )
        /// </summary>
        public int? ProductParentId { get; set; }

        /// <summary>
        /// 商品類別名稱
        /// </summary>
        public string ProductCategoryName { get; set; }
    }
}
