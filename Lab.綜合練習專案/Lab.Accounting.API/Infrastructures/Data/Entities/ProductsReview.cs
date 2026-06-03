namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class ProductsReview
    {
        /// <summary>
        /// 審核紀錄 ID
        /// </summary>
        public int ProductsReviewId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        public int? ProductsId { get; set; }

        /// <summary>
        /// 申請審核的賣家 ID
        /// </summary>
        public int SellerId { get; set; }

        /// <summary>
        /// 負責審核的管理員 ID (待審核時為 null)
        /// </summary>
        public int? AdminId { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ProductsName { get; set; }

        /// <summary>
        /// 商品價格
        /// </summary>
        public decimal ProductsPrice { get; set; }

        /// <summary>
        /// 商品庫存數量
        /// </summary>
        public int ProductsStock { get; set; }

        /// <summary>
        /// 商品詳細描述
        /// </summary>
        public string ProductsDescription { get; set; }

        /// <summary>
        /// 商品類別 ID
        /// </summary>
        public int ProductCategoryId { get; set; }

        /// <summary>
        /// 審核狀態 (例如：0=待審核, 1=審核通過, 2=審核失敗)
        /// </summary>
        public ReviewStatusEnum ReviewStatus { get; set; }

        /// <summary>
        /// 審核未通過/駁回的原因
        /// </summary>
        public string NotPassReson { get; set; }

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
