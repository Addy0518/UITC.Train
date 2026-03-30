namespace Lab.API.TODO.Common.Requests
{
    public class LedgerInsertRequest
    {
        // 新增的請求格式
        /// <summary>
        /// 帳本項目名稱
        /// </summary>
        [Display(Name = "帳本項目名稱")]
        [Required(ErrorMessage = "{0} 必輸")]
        [MaxLength(200, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string ItemName { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 必輸")]
        public int UserId { get; set; }

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
        [Required(ErrorMessage = "{0} 必輸")]
        public Decimal ItemCost { get; set; }

        /// <summary>
        /// 帳本項目建立日期
        /// </summary>
        [Display(Name = "帳本項目建立日期")]
        public DateTime? ItemCreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 帳本項目詳細說明
        /// </summary>
        [Display(Name = "帳本項目詳細說明")]
        public string ItemIllustrate { get; set; }

        /// <summary>
        /// 是否為刪除狀態
        /// </summary>
        [Display(Name = "是否為刪除狀態")]
        public bool IsDelete { get; set; } = false;
    }
}
