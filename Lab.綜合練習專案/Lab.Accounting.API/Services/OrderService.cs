namespace Lab.Accounting.API.Services;

public class OrderService(
    IProductsRepository productsRepositories,
    IProductsImgRepository productsImgRepository,
    IProductsRateRepository productsRateRepositories,
    IProductsOrderRepository productsBuyRepositories
) : IOrderService
{
    /// <summary>
    /// 買家查看所有訂單
    /// </summary>
    /// <param name="userId">使用者 ID </param>
    /// <returns>訂單 ID</returns>
    public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetUserOrder(int userId)
    {
        var target = await productsBuyRepositories.GetUserOrder(userId);

        if (target == null)
        {
            return ApiResponseHelper.NotFound<IEnumerable<OrderResponse>>();
        }

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 買家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="userId">使用者 ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<ApiResponse<OrderResponse>> GetUserOneOrder(int orderId, int userId)
    {
        var target = await productsBuyRepositories.GetUserOneOrder(orderId, userId);

        if (target == null)
        {
            return ApiResponseHelper.NotFound<OrderResponse>();
        }

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 賣家查看所有訂單
    /// </summary>
    /// <param name="userId">使用者 ID</param>
    /// <returns>所有訂單資訊</returns>
    public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetSellerOrder(int userId)
    {
        var target = await productsBuyRepositories.GetSellerOrder(userId);

        if (target == null)
        {
            return ApiResponseHelper.NotFound<IEnumerable<OrderResponse>>();
        }

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 賣家查看單一訂單
    /// </summary>
    /// <param name="orderId">訂單 ID </param>
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>訂單資訊</returns>
    public async Task<ApiResponse<OrderResponse>> GetSellerOneOrder(int orderId, int sellerId)
    {
        var target = await productsBuyRepositories.GetSellerOneOrder(orderId, sellerId);

        if (target == null)
        {
            return ApiResponseHelper.NotFound<OrderResponse>();
        }

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 改變運輸狀態
    /// </summary>
    /// <param name="orderId">訂單 ID</param>
    /// <returns>影響行數</returns>
    public async Task<ApiResponse<int>> UpdateShippingStatus(int orderId, ShippingStatusEnum shippingStatus)
    {
        var target = await productsBuyRepositories.UpdateShippingStatus(orderId, shippingStatus);

        if (target <= 0)
        {
            return ApiResponseHelper.NotFound<int>();
        }

        return ApiResponseHelper.Success(target);
    }

    /// <summary>
    /// 使用者購買商品並跳轉綠界界面
    /// </summary>
    /// <param name="Request">商品購買資訊 </param>
    /// <returns>訂單 ID</returns>
    public async Task<ApiResponse<int>> UserBuyProduct(ProductsBuyRequest Request)
    {
        using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
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

            var countStock = target.ProductsStock - Request.BoughtQuantity;
            var stock = await productsRepositories.SetStock(Request.ProductsId, countStock);

            if (stock <= 0)
                return ApiResponseHelper.InternalException<int>("庫存更新失敗");

            var buytarget = new MallOrder
            {
                OrderNumber = merchantTradeNo,
                UserId = Request.UserId,
                ProductsId = Request.ProductsId,
                BoughtQuantity = Request.BoughtQuantity,
                UnitPrice = target.ProductsPrice,
                BoughtTime = DateTime.Now,
                ShippingAddress = Request.ShippingAddress,
                ShippingStatus = (int)ShippingStatusEnum.PendingPayment,
            };
            var order = await productsBuyRepositories.BuyProducts(buytarget);
            trxScope.Complete();
            return ApiResponseHelper.Success(order);
        }
    }

    /// <summary>
    /// 綠界訂單創建(新增)
    /// </summary>
    /// <param name="orderId">商品購買資訊 </param>
    /// <param name="tunnelUrl">開發者通道網址</param>
    /// <returns>跳轉綠界訂單</returns>
    public async Task<ApiResponse<GreenPayResponse>> GetPaymentData(int orderId, int userId, string tunnelUrl)
    {
        var target = await productsBuyRepositories.GetUserOneOrder(orderId, userId);

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
            { "ReturnURL", $"{tunnelUrl}/api/Order/EcPayBack" },
            //交易完付款之後會呼叫的API(也就是我規定要呼叫哪個API,在下面)
            //return也是最重要的,因為他的用意就是更改資料庫狀態改為以付款(改的方法就寫在這個API裡)

            { "OrderResultURL", $"{tunnelUrl}/api/Order/PaymentCallback" },
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
    /// 接收綠界回傳資料(驗證)
    /// </summary>
    /// <param name="collection">綠界回傳的表單資料</param>
    /// <returns>回傳成功或失敗代號</returns>
    public async Task<string> SetPaymentData(IFormCollection collection)
    {
        //這裡很重要!要驗證傳回來的資料跟當初發過去的一樣,不然誰拿到這個API都可以呼叫了
        //先轉成字典(ToDictionary),因為剛剛發的訂單資料型態也是字典
        //這裡解釋一下寫法
        //第一個k=>k是遍歷所有字典的key(欄位),第二個k=>collection是拿到這些欄位的值
        var response = collection.Keys.ToDictionary(k => k, k => collection[k].ToString());

        //因為我們要驗證資料,所以要把舊的檢查碼踢掉(補充:檢查碼是所有資料加再一起算出來的,Helper有)
        response.Remove("CheckMacValue");

        //再把資料再計算一遍,重新生一個檢查碼,待會用來比對
        string MyCheckValue = ECPayHelper.GetCheckMacValue(response);
        //生成新的綠界的檢查碼
        string ezpay = collection["CheckMacValue"].ToString();

        if (MyCheckValue.Equals(ezpay, StringComparison.OrdinalIgnoreCase))
        //StringComparison.OrdinalIgnoreCase是忽略大小寫
        {
            //驗證成功!這是綠界傳來的不是其他地方傳的
            var rtnCode = collection["RtnCode"].ToString();

            //訂單成立之後,開始更新資料庫
            var orderNo = collection["MerchantTradeNo"].ToString();
            var buyProduct = await productsBuyRepositories.GetOrderByOrderNumber(orderNo);
            if (buyProduct == null)
            {
                return "0|OrderNotFound_CheckDB"; // 如果回傳這個，代表你傳給 Postman 的編號在資料庫找不到
            }
            if (buyProduct != null)
            {
                string tradeAmt = collection["TradeAmt"].ToString();

                if (!decimal.TryParse(tradeAmt, out decimal totalPrice))
                    return "0|InvalidTradeAmt";

                var totalAmount = buyProduct.UnitPrice * buyProduct.BoughtQuantity;

                if (totalPrice != totalAmount)
                    //金額不符,可能是資料被竄改了,不處理這筆訂單
                    return "0|InvalidAmount";

                DateTime.TryParse(collection["PaymentDate"], out DateTime paidTime);
                if (paidTime == DateTime.MinValue)
                {
                    paidTime = DateTime.Now; // 如果解析不到時間，就用系統現在時間
                }
                var paymentCompleted = await productsBuyRepositories.PaidProducts(
                    orderNo,
                    (int)ShippingStatusEnum.PendingShipment,
                    totalPrice,
                    collection["PaymentType"].ToString(),
                    paidTime
                );
                if (paymentCompleted <= 0)
                {
                    return "0|DBUpdateFailed";
                }
            }
            //因為綠界規定交易成功要回傳1跟ok

            return "1|OK";
        }
        else
        {
            //驗證失敗..丟掉
            return "0|CheckMacValueVerifyFail";
        }
    }
}
