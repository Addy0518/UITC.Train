namespace Lab.Accounting.API.Common.Requests.Logistics
{
    public class LogisticsOrderInfoRequest
    {
        /// <summary>
        /// 物流訂單編號
        /// </summary>
        public string MerchantTradeNo { get; set; } = string.Empty;

        /// <summary>
        /// 物流子類型 ( 例如 UNIMARTC2C：7-ELEVEN超商交貨便、FAMIC2C：全家店到店 )
        /// </summary>
        public string LogisticsSubType { get; set; } = string.Empty;

        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal GoodsAmount { get; set; }

        /// <summary>
        /// 商品名稱 ( 顯示在託運單上，多筆商品可自行組合成單一字串 )
        /// </summary>
        public string GoodsName { get; set; } = string.Empty;

        /// <summary>
        /// 寄件人姓名
        /// </summary>
        public string SenderName { get; set; } = string.Empty;

        /// <summary>
        /// 寄件人市話
        /// </summary>
        public string? SenderPhone { get; set; }

        /// <summary>
        /// 寄件人手機 ( 賣家填寫的收件手機，UNIMARTC2C / HILIFEC2C 這兩種子類型必填 )
        /// </summary>
        public string? SenderCellPhone { get; set; }

        /// <summary>
        /// 寄件人郵遞區號 ( 宅配 )
        /// </summary>
        public string? SenderZipCode { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收件人市話
        /// </summary>
        public string? ReceiverPhone { get; set; }

        /// <summary>
        /// 收件人手機 ( 買家填寫的收件手機，UNIMARTC2C / HILIFEC2C 這兩種子類型必填 )
        /// </summary>
        public string? ReceiverCellPhone { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string? ReceiverAddress { get; set; }

        /// <summary>
        /// 收件人郵遞區號 ( 宅配 )
        /// </summary>
        public string? ReceiverZipCode { get; set; }

        /// <summary>
        /// 收件門市代號 ( 對應 OrderLogistics.StoreCode，也就是買家選門市時存下來的 CVSStoreID )
        /// </summary>
        public string ReceiverStoreID { get; set; } = string.Empty;
    }
}
