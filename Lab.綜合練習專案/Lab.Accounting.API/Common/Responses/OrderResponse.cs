namespace Lab.Accounting.API.Common.Responses
{
    public class OrderResponse
    {
        /// <summary>
        /// 訂單 ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 賣家 ID
        /// </summary>
        public int SellerUserId { get; set; }

        /// <summary>
        /// 賣家名稱
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 購買者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 購買者名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        public int ProductsId { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ProductsName { get; set; }

        /// <summary>
        /// 商品類別 ID
        /// </summary>
        public int ProductCategoryId { get; set; }

        /// <summary>
        /// 購買數量
        /// </summary>
        public int BoughtQuantity { get; set; }

        /// <summary>
        /// 原始單品價格
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 原始總價
        /// </summary>
        public decimal OrginalAmount { get; set; }

        /// <summary>
        /// 被折扣多少
        /// </summary>
        public decimal? PlatformDiscount { get; set; }

        /// <summary>
        /// 最終總價
        /// </summary>
        public decimal AccountAmount { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string? PaidType { get; set; }

        /// <summary>
        /// 付款時間
        /// </summary>
        public DateTime? PaidTime { get; set; }

        /// <summary>
        /// 購買時間
        /// </summary>
        public DateTime BoughtTime { get; set; }

        /// <summary>
        /// 運送地址
        /// </summary>
        public string? ShippingAddress { get; set; }

        /// <summary>
        /// 運送狀態
        /// </summary>
        public ShippingStatusEnum ShippingStatus { get; set; }

        /// <summary>
        /// 商品圖片 URL
        /// </summary>
        public string ProductsImg { get; set; }

        /// <summary>
        /// 是否為刪除狀態
        /// </summary>
        public IsDeleteStatusEnum? IsDelete { get; set; }

        /// <summary>
        /// 總筆數
        /// </summary>
        public int? TotalCount { get; set; }
    }
}
