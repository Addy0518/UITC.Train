namespace Lab.Accounting.API.Common.Requests.Store
{
    public class StoreRegisterRequest
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int UserId { get; set; }

        /// <summary>
        /// 賣場名稱
        /// </summary>
        [Display(Name = "賣家名稱")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string StoreName { get; set; }

        /// <summary>
        /// 建立日期
        /// </summary>
        [Display(Name = "建立日期")]
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新日期
        /// </summary>
        [Display(Name = "更新日期")]
        public DateTime? UpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 是否啟用
        /// </summary>
        [Display(Name = "是否啟用")]
        public IsDeleteStatusEnum? IsDelete { get; set; } = IsDeleteStatusEnum.Normal;
    }
}
