namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class OrderLogisticsTemp
    {
        /// <summary>
        /// 物流暫存單 ID
        /// </summary>
        public int LogisticsTempId { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        public string SessionKey { get; set; }

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
        /// 收件人地址郵遞區號 (宅配才需要)
        /// </summary>
        public string? ReceiverZipCode { get; set; }

        /// <summary>
        /// 過期時間 ( 未付款自動過期 )
        /// </summary>
        public DateTime ExpiredAt { get; set; }
    }
}
