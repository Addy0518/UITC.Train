using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IProductsImgRepository
    {
        /// <summary>
        /// 商品圖片上傳
        /// </summary>
        /// <param name="productImgs">多個商品圖片檔案</param>
        /// <returns>影響列數</returns>
        Task<int> ProductsImgUpload(IEnumerable<ProductImg> productImgs);

        /// <summary>
        /// 查看商品圖片
        /// </summary>
        /// <param name="productsId">商品 ID </param>
        /// <returns>商品圖片 URL</returns>
        Task<IEnumerable<string>> GetProductsImg(int productsId);
    }
}
