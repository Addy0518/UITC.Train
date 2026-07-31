namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class OrderReturn
    {
        /// <summary>
        /// 退貨單 ID
        /// </summary>
        public int OrderReturnId { get; set; }

        /// <summary>
        /// 訂單 ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 申請退貨的買家 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 退貨原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// 退貨狀態
        /// </summary>
        public OrderReturnStatusEnum ReturnStatus { get; set; }

        /// <summary>
        /// 賣家審核退貨的回覆 ( 同意/拒絕原因 )
        /// </summary>
        public string? SellerReplyReason { get; set; }

        /// <summary>
        /// 買家寄回時填寫的物流追蹤編號 ( 手動輸入，不透過綠界 API 自動產生 )
        /// </summary>
        public string? ReturnTrackingNo { get; set; }

        /// <summary>
        /// 退款金額
        /// </summary>
        public decimal? RefundAmount { get; set; }

        /// <summary>
        /// 綠界退款用的交易編號 ( 對應 SetPaymentData 收到的 TradeNo )
        /// </summary>
        public string? EcpayTradeNo { get; set; }

        /// <summary>
        /// 申請時間
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
