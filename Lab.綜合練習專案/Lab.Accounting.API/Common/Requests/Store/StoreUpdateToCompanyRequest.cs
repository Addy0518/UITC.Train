namespace Lab.Accounting.API.Common.Requests.Store
{
    public class StoreUpdateToCompanyRequest
    {
        /// <summary>
        /// 賣場 ID
        /// </summary>
        [Display(Name = "賣場 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int StoreId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int UserId { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        [Display(Name = "公司名稱")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string StoreCompanyName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        [Display(Name = "統一編號")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "統編必須為 8 個數字")]
        public string StoreUnifiedNumber { get; set; }

        /// <summary>
        /// 營業登記證明文件路徑
        /// </summary>
        [Display(Name = "營業登記證明文件")]
        public string? DocumentPath { get; set; }

        /// <summary>
        /// 審核狀態 (例如：0=待審核, 1=審核通過, 2=審核失敗)
        /// </summary>
        [Display(Name = "審核狀態")]
        public ReviewStatusEnum? ReviewStatus { get; set; }

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
    }
}
