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
        IProductsRateRepositories productsRateRepositories,
        IProductsShoppingCarRepositories productsShoppingCarRepositories,
        IWebHostEnvironment env
    ) : IMallService
    {
        /// <summary>
        /// 賣家查看商品
        /// </summary>
        /// <param name="productId">商品 Id</param>
        /// <returns>商品資訊</returns>
        public async Task<ApiResponse<ProductsResponse>> GetProducts(int productId)
        {
            var target = await productsRepositories.GetProducts(productId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<ProductsResponse>();
            }
            var imgs = await productsImgRepository.GetProductsAllImg(productId);
            target.ProductsImgs = imgs;

            var avgRating = await productsRateRepositories.CountAVGProductRate(productId);
            target.ProductsRate = avgRating;

            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 查看所有商品 ( 可選擇查看指定賣家的所有商品 )
        /// </summary>
        /// <param name="pageIndex">頁碼</param>
        /// <param name="pageSize">每頁顯示數量</param>
        /// <param name="userId">使用者 Id</param>
        /// <param name="isDelete">是否為刪除狀態</param>
        /// <returns>商品列表</returns>
        public async Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProducts(
            int pageIndex,
            int pageSize,
            int? userId = null,
            bool? isDelete = false
        )
        {
            var products = await productsRepositories.GetAllProducts(pageIndex, pageSize, userId, isDelete);

            if (products == null)
            {
                return ApiResponseHelper.NotFound<IEnumerable<ProductsResponse>>();
            }

            foreach (var product in products)
            {
                var avgRating = await productsRateRepositories.CountAVGProductRate(product.ProductsId);
                product.ProductsRate = avgRating;
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
                ProductsStock = productsInsertRequest.ProductsStock,
                UserId = productsInsertRequest.UserId,
                IsDelete = false,
            };
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var target = await productsRepositories.CreateProducts(product);
                foreach (var categoryId in productsInsertRequest.ProductCategoryId)
                {
                    var productCategoryTarget = await productsCategoryRepositories.CreateProductsCategory(
                        target,
                        categoryId
                    );
                }
                var Insertrate = new MallProductsRate
                {
                    ProductsId = target,
                    UserId = productsInsertRequest.UserId,
                    Comment = null,
                    CreateTime = DateTime.UtcNow,
                    Rating = 3,
                };
                var rating = await productsRateRepositories.CreateProductRate(Insertrate);

                trxScope.Complete();
                return ApiResponseHelper.Success(target);
            }
        }

        /// <summary>
        /// 更新單一商品
        /// </summary>
        /// <param name="productsUpdateRequest">商品更新資訊</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UpdateProducts(ProductsUpdateRequest productsUpdateRequest)
        {
            var updateTarget = new MallProducts
            {
                UserId = productsUpdateRequest.UserId,
                ProductsId = productsUpdateRequest.ProductsId,
                ProductsName = productsUpdateRequest.ProductsName,
                ProductsPrice = productsUpdateRequest.ProductsPrice,
                ProductsStock = productsUpdateRequest.ProductsStock,
            };
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var target = await productsRepositories.GetProducts(productsUpdateRequest.ProductsId);
                if (target == null || target.UserId != productsUpdateRequest.UserId)
                {
                    return ApiResponseHelper.NotFound<int>();
                }
                var result = await productsRepositories.UpdateProducts(updateTarget);
                await productsCategoryRepositories.DeleteProductsCategory(productsUpdateRequest.ProductsId);
                foreach (var categoryId in productsUpdateRequest.ProductCategoryId)
                {
                    var productCategoryTarget = await productsCategoryRepositories.CreateProductsCategory(
                        productsUpdateRequest.ProductsId,
                        categoryId
                    );
                }
                trxScope.Complete();
                return ApiResponseHelper.Success(result);
            }
        }

        /// <summary>
        /// 復原已選取的商品刪除狀態
        /// </summary>
        /// <param name="productId">選取的所有商品 Id</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UpdateProductsDeleteStatus(int userId, IEnumerable<int> productId)
        {
            var target = await productsRepositories.UpdateProductsDeleteStatus(userId, productId);
            if (target == 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 軟刪除或硬刪除單一商品
        /// </summary>
        /// <param name="productsId">商品 ID</param>
        /// <param name="userId">使用者 ID</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> DeleteProducts(int productsId, int userId)
        {
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var target = await productsRepositories.GetProducts(productsId);
                if (target == null || target.UserId != userId)
                {
                    return ApiResponseHelper.NotFound<int>();
                }

                IEnumerable<MallProductImg> imgs = new List<MallProductImg>();
                if (target.IsDelete == true)
                {
                    imgs = await productsImgRepository.GetProductsAllImg(productsId);
                }

                var deletetarget = await productsRepositories.DeleteProducts(productsId, target.IsDelete, userId);
                if (target.IsDelete == true && deletetarget != null)
                {
                    foreach (var img in imgs)
                    {
                        FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsImg", img.ProductsImg);
                    }
                }

                trxScope.Complete();
                return ApiResponseHelper.Success<int>(deletetarget);
            }
        }

        /// <summary>
        /// 商品圖片上傳
        /// </summary>
        /// <param name="productsImgsFiles">商品圖片檔案</param>
        /// <param name="productId">商品 ID</param>
        /// <returns>新增成功的圖片</returns>
        public async Task<ApiResponse<IEnumerable<MallProductImg>>> ProductsImgUpload(
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
        /// 刪除商品圖片
        /// </summary>
        /// <param name="productsImgId">商品圖片 ID</param>
        /// <returns>刪除的圖片</returns>
        public async Task<ApiResponse<MallProductImg>> DeleteProductsImg(int productsImgId)
        {
            var result = await productsImgRepository.DeleteProductsImg(productsImgId);
            if (result == null)
            {
                return ApiResponseHelper.NotFound<MallProductImg>();
            }
            FileUploadHelper.DeleteFile(env.WebRootPath, "ProductsImg", result.ProductsImg);
            return ApiResponseHelper.Success(result);
        }

        /// <summary>
        /// 使用者購買商品並評分
        /// </summary>
        /// <param name="Request">商品購買資訊 </param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> UserBuyProductAndRate(ProductsBuyRequest Request)
        {
            var target = await productsRepositories.GetProducts(Request.ProductsId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var buytarget = await productsRepositories.BuyProducts(
                    Request.ProductsId,
                    Request.UserId,
                    Request.PurchaseQuantity
                );
                var remainStock = 0;
                if (target.ProductsStock >= Request.PurchaseQuantity)
                {
                    remainStock = target.ProductsStock - Request.PurchaseQuantity;
                }
                else
                {
                    var errors = new Dictionary<string, string[]> { { "ProductsStock", new[] { "庫存不足!" } } };

                    return ApiResponseHelper.RequestError<int>(errors);
                }

                var stocktarget = await productsRepositories.SetStock(Request.ProductsId, remainStock);

                var Insertrate = new MallProductsRate
                {
                    ProductsId = Request.ProductsId,
                    UserId = Request.UserId,
                    Comment = Request.Comment,
                    CreateTime = DateTime.UtcNow,
                    Rating = Request.Rating,
                };

                var ratetarget = await productsRateRepositories.CreateProductRate(Insertrate);

                trxScope.Complete();

                return ApiResponseHelper.Success(ratetarget);
            }
        }

        /// <summary>
        /// 查看購物車中的所有商品
        /// </summary>
        /// <param name="userId">使用者 Id</param>
        /// <returns>購物車中的所有商品</returns>
        public async Task<ApiResponse<IEnumerable<ProductsResponse>>> GetAllProductsInShoppingCar(int userId)
        {
            var alltarget = await productsShoppingCarRepositories.GetAllProductsInShoppingCar(userId);
            if (alltarget == null || !alltarget.Any())
            {
                return ApiResponseHelper.NotFound<IEnumerable<ProductsResponse>>();
            }
            foreach (var target in alltarget)
            {
                var imgs = await productsImgRepository.GetProductsAllImg(target.ProductsId);
                target.ProductsImgs = imgs;
            }
            return ApiResponseHelper.Success<IEnumerable<ProductsResponse>>(alltarget);
        }

        /// <summary>
        /// 新增單一商品到購物車
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> AddProductsInShoppingCar(int productsId, int userId)
        {
            var target = await productsShoppingCarRepositories.AddProductsInShoppingCar(productsId, userId);
            return ApiResponseHelper.Success(target);
        }

        /// <summary>
        /// 刪除單一商品從購物車
        /// </summary>
        /// <param name="productsId">商品 Id</param>
        /// <param name="userId">使用者 Id</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> DeleteProductsInShoppingCar(int productsId, int userId)
        {
            var target = await productsShoppingCarRepositories.DeleteProductsInShoppingCar(productsId, userId);
            if (target == 0)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            return ApiResponseHelper.Success(target);
        }
    }
}
