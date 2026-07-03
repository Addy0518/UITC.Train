namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class OrderLogistics
    {
        /// <summary>
        /// 物流單 ID
        /// </summary>
        public int LogisticsId { get; set; }

        /// <summary>
        /// 訂單 ID
        /// </summary>
        public int OrderId { get; set; }

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
        /// 物流訂單編號 (傳給綠界用)
        /// </summary>
        public string MerchantTradeNo { get; set; }

        /// <summary>
        /// 綠界回傳的物流追蹤編號
        /// </summary>
        public string LogisticsTrackingNo { get; set; }

        /// <summary>
        /// 物流狀態 (Created / Shipped / InTransit / Delivered / PickedUp / Cancelled)
        /// </summary>
        public LogisticsStatusEnum LogisticsStatus { get; set; }

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
    }
}
