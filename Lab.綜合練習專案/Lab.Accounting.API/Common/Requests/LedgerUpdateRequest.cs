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
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int UserId { get; set; }

        /// <summary>
        /// 帳本項目名稱
        /// </summary>
        [Display(Name = "項目名稱")]
        [MaxLength(200, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string? ItemName { get; set; }

        /// <summary>
        /// 帳本項目名稱
        /// </summary>
        [Display(Name = "帳本項目名稱")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string CategoryName { get; set; }

        /// <summary>
        /// 帳本項目花費
        /// </summary>
        [Display(Name = "帳本項目花費")]
        public Decimal ItemCost { get; set; }

        /// <summary>
        /// 帳本項目更新日期
        /// </summary>
        [Display(Name = "帳本項目更新日期")]
        public DateTime ItemUpdateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 詳細說明
        /// </summary>
        [Display(Name = "詳細說明")]
        public string ItemIllustrate { get; set; }

        /// <summary>
        /// 是否為刪除狀態
        /// </summary>
        [Display(Name = "是否為刪除狀態")]
        public bool isDelete { get; set; }
    }
}
