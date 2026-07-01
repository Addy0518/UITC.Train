namespace Lab.Accounting.API.Common.Requests.Order
{
    // 綠界回傳門市資料
    public class CvsStoreCallbackRequest
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
        /// 物流方式 (CVS: 超商)
        /// </summary>
        public string LogisticsType { get; set; } = string.Empty;

        /// <summary>
        /// 物流子類型 (UNIMART: 7-11 / FAMIC2C: 全家 / HILIFEC2C: 萊爾富)
        /// </summary>
        public string LogisticsSubType { get; set; } = string.Empty;

        /// <summary>
        /// 超商門市代號
        /// </summary>
        public string CVSStoreID { get; set; } = string.Empty;

        /// <summary>
        /// 超商門市名稱
        /// </summary>
        public string CVSStoreName { get; set; } = string.Empty;

        /// <summary>
        /// 超商門市地址
        /// </summary>
        public string CVSAddress { get; set; } = string.Empty;

        /// <summary>
        /// 超商門市電話
        /// </summary>
        public string? CVSTelephone { get; set; } = string.Empty;

        /// <summary>
        /// 自訂備用資料（用來帶 SessionKey / MerchantTradeNo）
        /// </summary>
        public string ExtraData { get; set; } = string.Empty;
    }
}
