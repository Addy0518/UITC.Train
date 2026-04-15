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
    }
}
