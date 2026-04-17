namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class MallShoppingCar
    {
        /// <summary>
        /// 購物車 ID
        /// </summary>
        public int ShoppingCarId { get; set; }

        /// <summary>
        /// 使用者 ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 商品 ID
        /// </summary>
        public int ProductsId { get; set; }
    }
}
