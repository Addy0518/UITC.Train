namespace Lab.Accounting.API.Common.Requests.Order
{
    // 綠界物流狀態異動通知
    public class LogisticsStatusCallbackRequest
    {
        /// <summary>
        /// 綠界特店編號
        /// </summary>
        public string MerchantID { get; set; } = string.Empty;

        /// <summary>
        /// 物流訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; } = string.Empty;

        /// <summary>
        /// 綠界物流追蹤編號（建立物流訂單成功後綠界回傳的編號）
        /// </summary>
        public string AllPayLogisticsID { get; set; } = string.Empty;

        /// <summary>
        /// 物流狀態代碼（例如 "300" 建立成功 / "3024" 送達門市 / "3032" 買家已取件）
        /// </summary>
        public string LogisticsStatus { get; set; } = string.Empty;

        /// <summary>
        /// 商品金額
        /// </summary>
        public string GoodsAmount { get; set; } = string.Empty;

        /// <summary>
        /// 狀態更新時間（綠界格式：yyyy/MM/dd HH:mm:ss）
        /// </summary>
        public string UpdateStatusDate { get; set; } = string.Empty;

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收件人電話
        /// </summary>
        public string ReceiverPhone { get; set; } = string.Empty;

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ReceiverAddress { get; set; } = string.Empty;

        /// <summary>
        /// 狀態說明文字（綠界回傳的描述，例如「成功」「配送中」）
        /// </summary>
        public string RtnMsg { get; set; } = string.Empty;
    }
}
