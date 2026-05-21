namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class Store
    {
        /// <summary>
        /// 賣場 ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 賣場名稱
        /// </summary>
        public string StoreName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        public string StoreUnifiedNumber { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        public string? StoreCompanyName { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 是否為刪除狀態
        /// </summary>
        public IsDeleteStatusEnum IsDelete { get; set; }
    }
}
