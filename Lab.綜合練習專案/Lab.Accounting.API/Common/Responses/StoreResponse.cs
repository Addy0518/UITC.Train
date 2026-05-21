namespace Lab.Accounting.API.Common.Responses
{
    public class StoreResponse
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 賣場名稱
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        public string StoreUnifiedNumber { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        public string? StoreCompanyName { get; set; }

        /// <summary>
        /// 所有商品的數量
        /// </summary>
        public int? AllProductsCount { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
