using Lab.Accounting.API.Infrastructures.Data.Entities;

namespace Lab.Accounting.API.Repositories.Interface
{
    public interface IProductsImgRepository
    {
        /// <summary>
        /// 商品圖片上傳  ( 判斷是否已有照片 )
        /// </summary>
        /// <param name="productsImgs">圖片</param>
        /// <param name="productId">商品 ID</param>
        /// <returns>影響列數</returns>
        Task<int> ProductsImgUpload(int productId, string productsImgs);

        /// <summary>
        /// 商品圖片更新
        /// </summary>
        /// <param name="productsImgs">圖片</param>
        /// <param name="productImgId">商品圖片 ID</param>
        /// <param name="productId">商品 ID</param>
        /// <returns>影響列數</returns>
        Task<int> ProductsImgUpdate(int productId, string productsImgs, int productImgId);

        /// <summary>
        /// 查看商品所有圖片
        /// </summary>
        /// <param name="productsId">商品 ID </param>
        /// <returns>商品圖片 URL</returns>
        Task<IEnumerable<MallProductImg>> GetProductsAllImg(int productsId);

        /// <summary>
        /// 查看商品圖片
        /// </summary>
        /// <param name="productsImgId">商品圖片 ID</param>
        /// <returns>商品圖片 URL</returns>
        Task<MallProductImg> GetProductsImg(int productsImgId);

        /// <summary>
        /// 刪除商品圖片
        /// </summary>
        /// <param name="productsId">商品 ID </param>
        /// <returns>影響列數</returns>
        Task<int> DeleteProductsImg(int productsId);
    }
}
