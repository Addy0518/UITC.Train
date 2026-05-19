namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class Seller
    {
        /// <summary>
        /// 賣家 ID
        /// </summary>
        public int SellerId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 賣家名稱
        /// </summary>
        public string SellerName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        public int SellerUnifiedNumber { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        public string? SellerCompanyName { get; set; }

        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
