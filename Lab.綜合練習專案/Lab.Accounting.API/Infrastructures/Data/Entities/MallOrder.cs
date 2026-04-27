namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class MallOrder
    {
        /// <summary>
        /// 訂單 ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        public int ProductsId { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public int BoughtQuantity { get; set; }

        /// <summary>
        /// 原始價格
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 最終價格
        /// </summary>
        public decimal? AccountPrice { get; set; }

        /// <summary>
        /// 購買時間
        /// </summary>
        public DateTime BoughtTime { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string? PaidType { get; set; }

        /// <summary>
        /// 付款時間
        /// </summary>
        public DateTime? PaidTime { get; set; }

        /// <summary>
        /// 運送地址
        /// </summary>
        public string? ShippingAddress { get; set; }

        /// <summary>
        /// 運送狀態
        /// </summary>
        public int ShippingStatus { get; set; }
    }
}
