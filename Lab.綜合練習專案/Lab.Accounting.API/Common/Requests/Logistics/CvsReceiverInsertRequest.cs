namespace Lab.Accounting.API.Common.Requests.Logistics
{
    public class CvsReceiverInsertRequest
    {
        /// <summary>
        /// 物流訂單編號
        /// </summary>
        [Display(Name = "物流訂單編號")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string SessionKey { get; set; } = string.Empty;

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
    }
}
