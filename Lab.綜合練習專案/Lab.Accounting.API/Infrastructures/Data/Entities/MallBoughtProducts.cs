namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class MallBoughtProducts
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        public int ProductsId { get; set; }

        /// <summary>
        /// 購買時間
        /// </summary>
        public DateTime BoughtTime { get; set; }
    }
}
