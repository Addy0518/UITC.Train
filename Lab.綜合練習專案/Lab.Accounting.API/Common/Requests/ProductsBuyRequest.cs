namespace Lab.Accounting.API.Common.Requests
{
    public class ProductsBuyRequest
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int ProductsId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 商品購買數量
        /// </summary>
        public int PurchaseQuantity { get; set; }

        /// <summary>
        /// 評分
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// 評論
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 發表時間
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
