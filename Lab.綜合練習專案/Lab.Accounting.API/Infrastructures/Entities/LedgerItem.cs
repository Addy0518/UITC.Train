namespace Lab.Accounting.API.Infrastructures.Entities
{
    public class LedgerItem
    {
        /// <summary>
        /// 項目 ID
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 項目名稱
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 類別 ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 花費
        /// </summary>
        public Decimal Cost { get; set; }

        /// <summary>
        /// 項目更新日期
        /// </summary>
        public DateTime? ItemUpdateDate { get; set; }

        /// <summary>
        /// 項目建立日期
        /// </summary>
        public DateTime ItemCreateDate { get; set; }

        /// <summary>
        /// 詳細說明
        /// </summary>
        public string? Illustrate { get; set; }

        /// <summary>
        /// 是否為刪除狀態
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
