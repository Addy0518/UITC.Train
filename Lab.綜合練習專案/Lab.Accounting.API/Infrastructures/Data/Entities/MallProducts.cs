namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class MallProducts
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int ProductsId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

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
        /// 商品購買人
        /// </summary>
        public int ProductsPurchasedUser { get; set; }

        /// <summary>
        /// 是否為刪除狀態
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
