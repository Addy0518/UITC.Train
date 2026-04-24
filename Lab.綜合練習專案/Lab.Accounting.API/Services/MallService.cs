using Lab.Accounting.API.Common.Helpers;
using Lab.Accounting.API.Common.Requests;
using Lab.Accounting.API.Common.Responses;
using Lab.Accounting.API.Infrastructures.Data.Entities;
using Lab.Accounting.API.Repositories.Interface;
using Lab.Accounting.API.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Asn1.X509;
using prjGonowWebApi.Areas.Company.Helper;

namespace Lab.Accounting.API.Services
{
    public class MallService(
        IProductsRepositories productsRepositories,
        IProductsImgRepository productsImgRepository,
        IProductsRateRepositories productsRateRepositories,
        IProductsShoppingCarRepositories productsShoppingCarRepositories,
        IProductsBuyRepositories productsBuyRepositories,
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
        /// 查看商品類別
        /// </summary>
        /// <param name="productcategoryId">商品類別 ID</param>
        /// <returns>商品類別</returns>
        public async Task<ApiResponse<IEnumerable<MallProductCategory>>> GetCategory(int? productcategoryId = null)
        {
            var target = await productsRepositories.GetCategory(productcategoryId);
            if (target == null)
            {
                return ApiResponseHelper.NotFound<IEnumerable<MallProductCategory>>();
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
                ProductsStock = productsInsertRequest.ProductsStock,
                ProductCategoryId = productsInsertRequest.ProductCategoryId,
                UserId = productsInsertRequest.UserId,
                IsDelete = false,
            };
            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var target = await productsRepositories.CreateProducts(product);

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
                ProductCategoryId = productsUpdateRequest.ProductCategoryId,
            };

            var target = await productsRepositories.GetProducts(productsUpdateRequest.ProductsId);
            if (target == null || target.UserId != productsUpdateRequest.UserId)
            {
                return ApiResponseHelper.NotFound<int>();
            }
            var result = await productsRepositories.UpdateProducts(updateTarget);

            return ApiResponseHelper.Success(result);
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
        /// 使用者購買商品並跳轉綠界界面
        /// </summary>
        /// <param name="Request">商品購買資訊 </param>
        /// <returns>訂單 ID</returns>
        public async Task<ApiResponse<int>> UserBuyProduct(ProductsBuyRequest Request)
        {
            var target = await productsRepositories.GetProducts(Request.ProductsId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<int>();
            }

            if (target.ProductsStock < Request.BoughtQuantity)
            {
                var errors = new Dictionary<string, string[]> { { "ProductsStock", new[] { "庫存不足!" } } };

                return ApiResponseHelper.RequestError<int>(errors);
            }

            //Guid是系統生成的"全球唯一識別碼",幾乎不會重複(像這樣=>550e8400-e29b-41d4-a716-446655440000)
            //但因為Guid生成時中間會有"-"這種符號,而綠界的訂單編號不允許一些特殊符號,所以轉成字串後replace把它拿掉
            //Substring切除剛剛的Guid碼,因為Guid字元會有32碼,太長了,所以只取前11碼
            string merchantTradeNo = "GN" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 11);

            var buytarget = new MallOrder
            {
                OrderNumber = merchantTradeNo,
                UserId = Request.UserId,
                ProductsId = Request.ProductsId,
                BoughtQuantity = Request.BoughtQuantity,
                UnitPrice = target.ProductsPrice,
                BoughtTime = DateTime.Now,
                ShippingAddress = Request.ShippingAddress,
                ShippingStatus = (int)ShippingStatus.PendingPayment,
            };
            var order = await productsBuyRepositories.BuyProducts(buytarget);

            return ApiResponseHelper.Success(order);
        }

        /// <summary>
        /// 綠界訂單創建(新增)
        /// </summary>
        /// <param name="orderId">商品購買資訊 </param>
        /// <param name="tunnelUrl">開發者通道網址</param>
        /// <returns>跳轉綠界訂單</returns>
        public async Task<ApiResponse<GreenPayResponse>> GetPaymentData(int orderId, int userId, string tunnelUrl)
        {
            var target = await productsBuyRepositories.GetOrder(orderId, userId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<GreenPayResponse>();
            }

            decimal totalAmount = target.UnitPrice * target.BoughtQuantity;
            var ecpay = new Dictionary<string, string>
            {
                { "MerchantID", "3002607" }, //這是測試用的商店編號,固定的
                { "MerchantTradeNo", target.OrderNumber },
                { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") }, //交易當下時間
                { "PaymentType", "aio" }, //付款類型=>全金流
                { "TotalAmount", totalAmount.ToString() },
                //先用int類型接收傳過來的變數,在轉成字串丟出去

                { "TradeDesc", "商品購買" }, // 交易類型
                { "ItemName", "商品名稱" }, // 交易名稱
                { "ReturnURL", $"{tunnelUrl}/api/Mall/ecpayback" },
                //交易完付款之後會呼叫的API(也就是我規定要呼叫哪個API,在下面)
                //return也是最重要的,因為他的用意就是更改資料庫狀態改為以付款(改的方法就寫在這個API裡)

                { "OrderResultURL", $"{tunnelUrl}/api/Mall/payment-callback" },
                //而order跟return不一樣的是,他是負責處理使用者付款完會跳轉的頁面
                //return是後端對後端,order是前端對前端

                { "ChoosePayment", "ALL" },
                //這是讓使用者有所有付款方式,當今天我想改成信用卡的話就寫"Credit"就好

                { "EncryptType", "1" },
                //這是固定寫法。代表我們要用 SHA256 方式加密（現在綠界強制規定都要用 1）
            };

            // 最後把這筆交易的檢查碼欄位加上我們建立的製作檢查方法
            ecpay["CheckMacValue"] = ECPayHelper.GetCheckMacValue(ecpay);

            // 回傳給 Angular，讓前端送出隱藏表單
            //如果沒有用表單,那就會變成直接發送一堆資料,安全性差
            //那為何要隱藏,因為我們只是借用html的表單提交功能跳轉,沒有要讓使用者還要填寫一個新表單
            //所以我們在前端建立一個隱藏的form,把資料都放在裡面,再把這些資料連同頁面跳轉到對方頁面(actionUrl)

            var result = new GreenPayResponse
            {
                FormData = ecpay,
                ActionUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5",
            };

            return ApiResponseHelper.Success(result);
        }

        /// <summary>
        /// 商品付款
        /// </summary>
        /// <param name="shippingStatus">運送狀態</param>
        /// <param name="accountPrice">最終金額</param>
        /// <param name="paidTime">付款時間</param>
        /// <returns>影響列數</returns>
        public async Task<ApiResponse<int>> PaidProducts(int shippingStatus, decimal accountPrice, DateTime paidTime)
        {
            return null;
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
