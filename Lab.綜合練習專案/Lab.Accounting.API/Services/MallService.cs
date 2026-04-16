using Lab.Accounting.API.Common.Helpers;
using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;
using Lab.Accounting.API.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509;

namespace Lab.Accounting.API.Services
{
    public class MallService(
        IProductsRepositories productsRepositories,
        IProductsCategoryRepositories productsCategoryRepositories,
        IProductsImgRepository productsImgRepository,
        IWebHostEnvironment env
    ) : IMallService
    {
        /// <summary>
        /// 查看單一商品
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>商品資訊</returns>
        public async Task<ApiResponse<ProductsResponse>> GetProducts(int productId, int userId)
        {
            var target = await productsRepositories.GetProducts(productId, userId);
            var imgs = await productsImgRepository.GetProductsAllImg(productId);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<ProductsResponse>();
            }
            target.ProductsImgs = imgs;
            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 查看所有商品 ( 分頁 )
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">每頁顯示數量</param>
        /// <returns>商品列表</returns>
        public async Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProducts(int pageIndex, int pageSize)
        {
            var products = await productsRepositories.GetAllProducts(pageIndex, pageSize);

            if (products == null)
            {
                return ApiResponseHelper.NotFound<IEnumerable<ProductsResponse>>();
            }

            foreach (var product in products)
            {
                var imgs = await productsImgRepository.GetProductsAllImg(product.ProductsId);
                product.ProductsImgs = imgs;
            }

            return ApiResponseHelper.Success(products);
        }

        /// <summary>
        /// 新增單一商品 + 類別
        /// </summary>
        /// <param name="productsInsertRequest">商品資訊</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> CreateProducts(ProductsInsertRequest productsInsertRequest)
        {
            var product = new MallProducts
            {
                ProductsName = productsInsertRequest.ProductsName,
                ProductsPrice = productsInsertRequest.ProductsPrice,
                UserId = productsInsertRequest.UserId,
            };
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var target = await productsRepositories.CreateProducts(product);

                var productTarget = await ExistsCategory(productsInsertRequest.ProductCategoryName);

                var productCategoryTarget = await productsCategoryRepositories.CreateProductsCategory(
                    target,
                    productTarget
                );

                trxScope.Complete();
                return ApiResponseHelper.Success(target);
            }
        }

        /// <summary>
        /// 商品圖片上傳
        /// </summary>
        /// <param name="productsImgsFiles">商品圖片檔案</param>
        /// <param name="productId">商品 Id</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<IEnumerable<ProductImg>>> ProductsImgUpload(
            IFormFile productsImgsFiles,
            int productId
        )
        {
            var result = await FileUploadHelper.SaveFileAsync(productsImgsFiles, env.WebRootPath, "ProductsImg");
            await productsImgRepository.ProductsImgUpload(productId, result);
            var newtarget = await productsImgRepository.GetProductsAllImg(productId);
            return ApiResponseHelper.Success(newtarget);
        }

        /// <summary>
        /// 商品圖片刪除
        /// </summa ry>
        /// <param name="productsImgId">商品圖片 ID</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> ProductsImgDelete(int productsImgId)
        {
            var target = await productsImgRepository.GetProductsImg(productsImgId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsImg", target.ProductsImg);
            int rows = await productsImgRepository.DeleteProductsImg(productsImgId);
            return ApiResponseHelper.Success(rows);
        }

        /// <summary>
        /// 私有方法 , 檢查商品類別是否存在
        /// </summary>
        /// <param name="productcategory">商品類別</param>
        /// <returns>影響列數</returns>
        private async Task<int> ExistsCategory(string productcategory)
        {
            int existcategory = 0;
            if (!string.IsNullOrWhiteSpace(productcategory))
            {
                // 查看類別是否存在
                existcategory = await productsCategoryRepositories.GetCategory(productcategory);
            }

            int productcategoryId = 0;
            // 存在就回傳 0 以上 , 直接塞 id
            if (existcategory > 0)
            {
                productcategoryId = existcategory;
            }
            // 不存在就創一個新類別 , 一樣塞 id 回去
            else
            {
                productcategoryId = await productsCategoryRepositories.CreateCategory(productcategory);
            }

            return productcategoryId;
        }
    }
}
