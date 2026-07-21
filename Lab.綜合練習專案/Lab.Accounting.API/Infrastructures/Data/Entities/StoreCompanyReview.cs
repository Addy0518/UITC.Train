namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class StoreCompanyReview
    {
        /// <summary>
        /// 賣場公司審核 ID
        /// </summary>
        public int StoreCompanyReviewId { get; set; }

        /// <summary>
        /// 對應的賣場 ID ( 這個賣場申請升級成企業賣場 )
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 申請人 ( 賣家 ) ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 申請的公司名稱
        /// </summary>
        public string StoreCompanyName { get; set; }

        /// <summary>
        /// 申請的統一編號
        /// </summary>
        public string StoreUnifiedNumber { get; set; }

        /// <summary>
        /// 上傳的營業登記證明文件路徑
        /// </summary>
        public string? DocumentPath { get; set; }

        /// <summary>
        /// 審核者 ( 管理員 ) ID
        /// </summary>
        public int? AdminId { get; set; }

        /// <summary>
        /// 審核狀態
        /// </summary>
        public ReviewStatusEnum ReviewStatus { get; set; }

        /// <summary>
        /// 未通過原因
        /// </summary>
        public string? NotPassReson { get; set; }

        /// <summary>
        /// 申請時間
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 審核時間
        /// </summary>
        public DateTime? ReviewTime { get; set; }
    }
}
