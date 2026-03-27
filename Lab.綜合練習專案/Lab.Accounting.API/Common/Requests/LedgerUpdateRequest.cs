namespace Lab.API.TODO.Common.Requests
{
    public class LedgerUpdateRequest
    {
        // 更新的請求格式
        /// <summary>
        /// 帳本項目ID
        /// </summary>
        [Display(Name = "項目ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int ItemId { get; set; }

        /// <summary>
        /// 帳本項目名稱
        /// </summary>
        [Display(Name = "項目名稱")]
        [Required(ErrorMessage = "{0} 必輸")]
        [MaxLength(200, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? ItemName { get; set; }

        /// <summary>
        /// 帳本項目類別 ID
        /// </summary>
        [Display(Name = "帳本項目類別 ID")]
        public string? CategoryId { get; set; }

        /// <summary>
        /// 帳本項目花費
        /// </summary>
        [Display(Name = "帳本項目花費")]
        [Required(ErrorMessage = "{0} 必輸")]
        public Decimal? ItemCost { get; set; }

        /// <summary>
        /// 帳本項目更新日期
        /// </summary>
        [Display(Name = "帳本項目更新日期")]
        public DateTime? ItemUpdateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 詳細說明
        /// </summary>
        [Display(Name = "詳細說明")]
        public string? ItemIllustrate { get; set; }
    }
}
