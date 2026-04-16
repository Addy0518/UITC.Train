using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Common.Responses
{
    public class ProductsResponse
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
        /// 商品類別名稱
        /// </summary>
        public string ProductCategoryName { get; set; }

        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ProductsName { get; set; }

        /// <summary>
        /// 商品價格
        /// </summary>
        public decimal ProductsPrice { get; set; }

        /// <summary>
        /// 商品圖片 URL
        /// </summary>
        public IEnumerable<ProductImg>? ProductsImgs { get; set; }
    }
}
