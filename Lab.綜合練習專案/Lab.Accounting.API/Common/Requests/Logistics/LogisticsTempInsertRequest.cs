namespace Lab.Accounting.API.Common.Requests.Order
{
    // 儲存配送資料到暫存表
    public class LogisticsTempInsertRequest
    {
        /// <summary>
        /// 物流訂單編號
        /// </summary>
        [Display(Name = "物流訂單編號")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string SessionKey { get; set; } = string.Empty;

        /// <summary>
        /// 物流方式 (CVS: 超商 / Home: 宅配)
        /// </summary>
        [Display(Name = "物流方式")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string LogisticsType { get; set; } = string.Empty;

        /// <summary>
        /// 物流子類型 (UNIMARTC2C: 7-ELEVEN超商交貨便 / FAMIC2C: 全家店到店 / HILIFEC2C：萊爾富店到店 / TCAT: 黑貓宅急便 等)
        /// </summary>
        [Display(Name = "物流子類型")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string LogisticsSubType { get; set; } = string.Empty;

        /// <summary>
        /// 超商門市代號（超商取貨才有，宅配填 null）
        /// </summary>
        [Display(Name = "門市代號")]
        [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? StoreCode { get; set; }

        /// <summary>
        /// 超商門市名稱（超商取貨才有，宅配填 null）
        /// </summary>
        [Display(Name = "門市名稱")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? StoreName { get; set; }

        /// <summary>
        /// 超商門市地址（超商取貨才有，宅配填 null）
        /// </summary>
        [Display(Name = "門市地址")]
        [MaxLength(200, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? StoreAddress { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        [Display(Name = "收件人姓名")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(50, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string ReceiverName { get; set; } = string.Empty;

        /// <summary>
        /// 收件人電話
        /// </summary>
        [Display(Name = "收件人電話")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(20, ErrorMessage = "{0} 長度最長為 {1} 字")]
        [RegularExpression(@"^09\d{8}$", ErrorMessage = "請符合手機號碼格式 0912345678")]
        public string ReceiverPhone { get; set; } = string.Empty;

        /// <summary>
        /// 收件人地址（宅配才有，超商取貨填 null）
        /// </summary>
        [Display(Name = "收件人地址")]
        [MaxLength(200, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? ReceiverAddress { get; set; }
    }
}
