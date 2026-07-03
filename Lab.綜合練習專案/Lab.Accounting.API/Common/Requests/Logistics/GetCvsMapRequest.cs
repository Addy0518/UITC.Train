namespace Lab.Accounting.API.Common.Requests.Order
{
    //  取得超商門市地圖網址
    public class GetCvsMapRequest
    {
        /// <summary>
        /// 物流訂單編號
        /// </summary>
        [Display(Name = "物流訂單編號")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string SessionKey { get; set; } = string.Empty;

        /// <summary>
        /// 物流子類型 (UNIMART: 7-11 / FAMIC2C: 全家 / HILIFEC2C: 萊爾富)
        /// </summary>
        [Display(Name = "物流子類型")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string LogisticsSubType { get; set; } = string.Empty;
    }
}
