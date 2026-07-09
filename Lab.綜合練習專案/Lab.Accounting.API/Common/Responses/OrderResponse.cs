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
        /// 物流單 ID
        /// </summary>
        public int? LogisticsId { get; set; }

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
        /// 物流方式 (CVS: 超商 / Home: 宅配)
        /// </summary>
        public string LogisticsType { get; set; }

        /// <summary>
        /// 物流子類型 (UNIMARTC2C: 7-ELEVEN超商交貨便 / FAMIC2C: 全家店到店 / HILIFEC2C：萊爾富店到店 / TCAT: 黑貓宅急便 等)
        /// </summary>
        public string LogisticsSubType { get; set; }

        /// <summary>
        /// 超商門市代號
        /// </summary>
        public string? StoreCode { get; set; }

        /// <summary>
        /// 超商門市名稱
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// 超商門市地址
        /// </summary>
        public string? StoreAddress { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人地址 (宅配才需要)
        /// </summary>
        public string? ReceiverAddress { get; set; }

        /// <summary>
        /// 物流狀態 (Created / Shipped / InTransit / Delivered / PickedUp / Cancelled)
        /// </summary>
        public LogisticsStatusEnum LogisticsStatus { get; set; }

        /// <summary>
        /// 綠界回傳的物流狀態碼
        /// </summary>
        public string LogisticsRtnCode { get; set; }

        /// <summary>
        /// 綠界回傳的物流狀態訊息
        /// </summary>
        public string LogisticsRtnMessage { get; set; }

        /// <summary>
        /// 物流資料建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 賣家出貨時間
        /// </summary>
        public DateTime? ShippedAt { get; set; }

        /// <summary>
        /// 送達門市或簽收時間
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// 買家完成取件時間
        /// </summary>
        public DateTime? PickedUpAt { get; set; }

        /// <summary>
        /// 總筆數
        /// </summary>
        public int? TotalCount { get; set; }
    }
}
