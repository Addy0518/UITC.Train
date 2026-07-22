namespace Lab.Accounting.API.Common.Requests.Store
{
    public class StoreReviewRequest
    {
        /// <summary>
        /// 賣場審核 ID
        /// </summary>
        [Display(Name = "賣場審核 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int StoreCompanyReviewId { get; set; }

        /// <summary>
        /// 管理員 ID
        /// </summary>
        [Display(Name = "管理員 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int AdminId { get; set; }

        /// <summary>
        /// 駁回理由
        /// </summary>
        [Display(Name = "駁回理由")]
        public string? NotPassReson { get; set; }

        /// <summary>
        /// 審核狀態 (例如：0=待審核, 1=審核通過, 2=審核失敗)
        /// </summary>
        [Display(Name = "審核狀態")]
        [Required(ErrorMessage = "{0} 必輸")]
        public ReviewStatusEnum ReviewStatus { get; set; }

        /// <summary>
        /// 審核日期
        /// </summary>
        [Display(Name = "審核日期")]
        public DateTime? ReviewTime { get; set; } = DateTime.Now;
    }
}
