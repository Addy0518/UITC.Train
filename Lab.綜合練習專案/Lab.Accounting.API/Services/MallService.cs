using Lab.Accounting.API.Common.Helpers;
using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;
using Lab.Accounting.API.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using NPOI.SS.Formula.Functions;

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
            if (target == null)
            {
                return ApiResponseHelper.NotFound<ProductsResponse>();
            }
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
            var target = await productsRepositories.GetAllProducts(pageIndex, pageSize);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<IEnumerable<ProductsResponse>>();
            }
            return ApiResponseHelper.Success(target);
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
        /// <param name="productImgs">多個商品圖片檔案</param>
        /// <returns>影響列數</returns>
        //public async Task<int> ProductsImgUpload(IFormFile[] productsImg, int productId)
        //{
        //    var imgTarget = new List<ProductImg>();
        //    var oldimg = await productsImgRepository.GetProductsImg(productId);
        //    foreach (var img in productsImg)
        //    {
        //        string fileUrl = await ExistFile(img, oldimg, "ProductsImg");

        //        imgTarget.Add(new ProductImg { ProductsId = productId, ProductsImg = fileUrl });
        //    }

        //    if (imgTarget.Count > 0)
        //    {
        //        await productsImgRepository.ProductsImgUpload(imgTarget);
        //    }
        //}

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

        /// <summary>
        /// 私有方法判斷文件是否存在
        /// </summary>
        /// <param name="newFile">新的檔案</param>
        /// <param name="oldPath">舊的檔案路徑</param>
        /// <param name="folder">檔案存放的資料夾</param>
        /// <returns>檔案路徑</returns>
        private async Task<string?> ExistFile(IFormFile? newFile, string? oldPath, string folder)
        {
            //沒更新就回傳舊檔案路徑
            if (newFile == null)
                return oldPath;

            //更新的話刪除舊檔案
            if (!string.IsNullOrEmpty(oldPath))
            {
                FileUploadHelper.DeleteFile(env.WebRootPath, folder, oldPath);
            }

            //不管怎樣都要儲存檔案
            return await FileUploadHelper.SaveFileAsync(newFile, env.WebRootPath, folder);
        }
    }
}
