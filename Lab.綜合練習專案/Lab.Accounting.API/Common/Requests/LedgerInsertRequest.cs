namespace Lab.API.TODO.Common.Requests
{
    public class InsertRequest
    {
        // 新增的請求格式
        /// <summary>
        /// 項目名稱
        /// </summary>
        [Display(Name = "項目名稱")]
        [Required(ErrorMessage = "{0} 必輸")]
        [MaxLength(200, ErrorMessage = "{0} 長度最長為 {1} 字")]
        public string ItemName { get; set; }

        /// <summary>
        /// 類別
        /// </summary>
        [Display(Name = "類別")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string Category { get; set; }

        /// <summary>
        /// 花費
        /// </summary>
        [Display(Name = "花費")]
        [Required(ErrorMessage = "{0} 必輸")]
        public string Cost { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Display(Name = "日期")]
        public string Date { get; set; }

        /// <summary>
        /// 詳細說明
        /// </summary>
        [Display(Name = "詳細說明")]
        public string Illustrate { get; set; }
    }
}
