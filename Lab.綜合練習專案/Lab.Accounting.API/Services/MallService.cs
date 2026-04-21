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
        /// <param name="userId">使用者 Id</param>
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
                foreach (var category in productsInsertRequest.ProductCategoryName)
                {
                    var productTarget = await ExistsCategory(category);
                    var productCategoryTarget = await productsCategoryRepositories.CreateProductsCategory(
                        target,
                        productTarget
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
        /// <param name="productId">商品 Id</param>
        /// <returns>影響列數</returns>
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

                var stocktarget = await productsRepositories.SetStock(Request.ProductsId, remainStock, Request.UserId);

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
