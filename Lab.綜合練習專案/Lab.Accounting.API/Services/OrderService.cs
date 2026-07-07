using Lab.Accounting.API.Common.Requests.Logistics;
using Lab.Accounting.API.Common.Requests.Order;
using Lab.Accounting.API.Common.Requests.Products;
using Org.BouncyCastle.Asn1.X509;

namespace Lab.Accounting.API.Services;

public class OrderService(
    IUserRepository userRepository,
    IProductsRepository productsRepositories,
    IProductsImgRepository productsImgRepository,
    IProductsRateRepository productsRateRepositories,
    IProductsOrderRepository productsBuyRepositories,
    IProductsShoppingCarRepository productsShoppingCarRepository,
    ILogisticsRepository logisticsRepository,
    ILogisticsTempRepository logisticsTempRepository,
    ILogisticsService logisticsService,
    ICouponRepository couponRepository
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
    /// <param name="sellerId">賣家 ID</param>
    /// <returns>所有訂單資訊</returns>
    public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetSellerOrder(int sellerId)
    {
        var target = await productsBuyRepositories.GetSellerOrder(sellerId);

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
    /// 查看所有訂單
    /// </summary>
    /// <param name="request">訂單搜尋請求</param>
    /// <returns>所有訂單資訊</returns>
    public async Task<ApiResponse<IEnumerable<OrderResponse>>> GetAllOrder(OrderSearchRequest request)
    {
        var target = await productsBuyRepositories.GetAllOrder(request);

        if (target == null)
        {
            return ApiResponseHelper.NotFound<IEnumerable<OrderResponse>>();
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
    /// <returns>多筆訂單 ID</returns>
    public async Task<ApiResponse<List<int>>> UserBuyProduct(ProductsBuyRequest Request)
    {
        List<int> orderIds = new List<int>();
        //Guid是系統生成的"全球唯一識別碼",幾乎不會重複(像這樣=>550e8400-e29b-41d4-a716-446655440000)
        //但因為Guid生成時中間會有"-"這種符號,而綠界的訂單編號不允許一些特殊符號,所以轉成字串後replace把它拿掉
        //Substring切除剛剛的Guid碼,因為Guid字元會有32碼,太長了,所以只取前11碼
        string merchantTradeNo = "GN" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 11);

        CouponResponse? coupon = null;
        CouponResponse? targetUserCoupon = null;

        // 原始總金額
        decimal totalOrginalAmount = 0;

        // 如果有使用優惠券，先驗證優惠券是否有效，再計算總金額是否達到門檻
        if (Request.CouponId > 0 && Request.CouponId.HasValue)
        {
            // 驗證優惠券是否存在且有效
            coupon = await couponRepository.GetCoupon(Request.CouponId.Value);

            if (coupon == null || !coupon.IsActive || DateTime.Now > coupon.EndTime)
            {
                var errors = new Dictionary<string, string[]> { { "Coupon", new[] { "優惠卷已過期或不存在!" } } };

                return ApiResponseHelper.RequestError<List<int>>(errors);
            }

            // 驗證使用者是否持有該優惠券且未使用
            var userCoupons = await couponRepository.GetUserCoupon(Request.UserId);
            targetUserCoupon = userCoupons.FirstOrDefault(uc =>
                uc.CouponId == Request.CouponId.Value && uc.UsedTime == null
            );
            if (targetUserCoupon == null || targetUserCoupon.UsedTime != null)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "Coupon", new[] { "你沒有這張優惠券，或該券已被使用 ! " } },
                };
                return ApiResponseHelper.RequestError<List<int>>(errors);
            }

            // 抓出購物車裡所有商品的原始總金額，來判斷是否達到優惠券使用門檻
            foreach (var product in Request.Products)
            {
                var item = await productsRepositories.GetProducts(product.ProductsId);
                if (item != null)
                {
                    totalOrginalAmount += item.ProductsPrice * product.BoughtQuantity;
                }
            }

            if (totalOrginalAmount < coupon.MinimunSpend)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "MinimunSpend", new[] { $"未達到優惠券使用門檻，最低消費金額為 {coupon.MinimunSpend} 元。" } },
                };
                return ApiResponseHelper.RequestError<List<int>>(errors);
            }
        }

        using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // 折扣的額度 , 防止固定金額折抵的優惠券在分攤到每件商品時出現小數點誤差，導致最後一件商品無法完全折抵完剩餘的金額 ( 最後一件商品直接扣不用平均 )
            decimal remainingDiscount = coupon != null ? coupon.Discount : 0;
            // 用來計算算到哪件商品了，判斷 remainingDiscount 分攤到最後一件商品時，直接把剩餘的折扣全扣掉
            int productCounter = 0;
            // 記錄每一筆訂單 , 用來新增對應的物流單
            var sellerMap = new List<(int orderId, int sellerId)>();
            // 開始處理每一件商品的訂單
            foreach (var product in Request.Products)
            {
                productCounter++;
                var target = await productsRepositories.GetProducts(product.ProductsId);

                if (target == null)
                {
                    return ApiResponseHelper.NotFound<List<int>>();
                }

                if (Request.UserId == target.UserId)
                {
                    var errors = new Dictionary<string, string[]>
                    {
                        { "UserId", new[] { "賣家沒辦法購買自己的商品!" } },
                    };

                    return ApiResponseHelper.RequestError<List<int>>(errors);
                }

                if (target.ProductsStock < product.BoughtQuantity)
                {
                    var errors = new Dictionary<string, string[]> { { "ProductsStock", new[] { "庫存不足!" } } };

                    return ApiResponseHelper.RequestError<List<int>>(errors);
                }

                var countStock = target.ProductsStock - product.BoughtQuantity;
                var stock = await productsRepositories.SetStock(product.ProductsId, countStock);
                if (stock <= 0)
                    return ApiResponseHelper.InternalException<List<int>>("庫存更新失敗");

                decimal orginalPrice = target.ProductsPrice * product.BoughtQuantity;
                decimal currentProductDiscount = 0;

                if (coupon != null)
                {
                    // 1. 百分比折抵邏輯
                    if (coupon.Type == (int)CouponTypeEnum.百分比折扣)
                    {
                        decimal discountRate = coupon.Discount / 100; // 80 / 100 = 0.8 (打 8 折)
                        currentProductDiscount = Math.Round(orginalPrice * (1 - discountRate), 0); // 算出這件商品折掉多少錢
                    }
                    // 2. 固定金額按比例分攤邏輯
                    else if (coupon.Type == (int)CouponTypeEnum.固定金額折抵 && remainingDiscount > 0)
                    {
                        if (productCounter < Request.Products.Count())
                        {
                            // 公式：(此商品原價 / 購物車總原價) * 優惠券總面額
                            currentProductDiscount = Math.Round(
                                (orginalPrice / totalOrginalAmount) * coupon.Discount,
                                0
                            );
                            remainingDiscount -= currentProductDiscount; // 扣掉已被分走的部分
                        }
                        else
                        {
                            // 最後一件商品直接拿走剩餘所有的折扣，完美防範小數點誤差 Bug
                            currentProductDiscount = remainingDiscount;
                            remainingDiscount = 0;
                        }
                    }
                }

                decimal accountPrice = orginalPrice - currentProductDiscount;
                if (accountPrice < 0)
                    accountPrice = 0;

                var buytarget = new Order
                {
                    OrderNumber = merchantTradeNo,
                    SellerUserId = target.UserId,
                    UserId = Request.UserId,
                    LogisticsId = null,
                    ProductsId = product.ProductsId,
                    ProductsName = target.ProductsName,
                    ProductCategoryId = target.ProductCategoryId,
                    BoughtQuantity = product.BoughtQuantity,
                    UnitPrice = target.ProductsPrice,
                    OrginalAmount = orginalPrice,
                    PlatformDiscount = currentProductDiscount,
                    AccountAmount = accountPrice,
                    BoughtTime = DateTime.Now,
                    ShippingStatus = (int)ShippingStatusEnum.PendingPayment,
                };
                var order = await productsBuyRepositories.BuyProducts(buytarget);

                orderIds.Add(order);
                sellerMap.Add((order, target.UserId));

                if (coupon != null && targetUserCoupon?.UserCouponId != null)
                {
                    await couponRepository.UpdateUserCoupon(order, targetUserCoupon.UserCouponId.Value);
                }

                await productsShoppingCarRepository.DeleteProductsInShoppingCar(product.ProductsId, Request.UserId);
            }

            // 根據賣家分訂單 , 假設同一筆訂單的兩件商品是一個賣家的 , 那就是一張物流單就好
            var sellerGroups = sellerMap.GroupBy(s => s.sellerId);
            // 用來生成物流單邊號末碼
            int shipmentIndex = 0;
            // 拿出暫存表塞入主表
            var orderLogisticsTemp = await logisticsTempRepository.GetLogisticsTemp(Request.SessionKey);

            if (orderLogisticsTemp == null)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "SessionKey", new[] { "配送資料已過期或不存在，請重新選擇配送方式" } },
                };
                return ApiResponseHelper.RequestError<List<int>>(errors);
            }

            foreach (var sellerGroup in sellerGroups)
            {
                shipmentIndex++;
                var logistics = new OrderLogistics
                {
                    LogisticsType = orderLogisticsTemp.LogisticsType,
                    LogisticsSubType = orderLogisticsTemp.LogisticsSubType,
                    StoreCode = orderLogisticsTemp.StoreCode ?? null,
                    StoreName = orderLogisticsTemp.StoreName ?? null,
                    StoreAddress = orderLogisticsTemp.StoreAddress ?? null,
                    ReceiverName = orderLogisticsTemp.ReceiverName,
                    ReceiverPhone = orderLogisticsTemp.ReceiverPhone,
                    ReceiverAddress = orderLogisticsTemp.ReceiverAddress ?? null,
                    MerchantTradeNo = $"{merchantTradeNo}-{shipmentIndex}",
                    LogisticsStatus = LogisticsStatusEnum.Created,
                    CreatedAt = DateTime.Now,
                };

                var logisticsId = await logisticsRepository.CreateLogistics(logistics);

                foreach (var (orderId, _) in sellerGroup)
                {
                    await productsBuyRepositories.UpdateLogisticsId(orderId, logisticsId);
                }
            }
            if (!string.IsNullOrEmpty(Request.SessionKey))
            {
                await logisticsTempRepository.DeleteBySessionKey(Request.SessionKey);
            }
            trxScope.Complete();
            return ApiResponseHelper.Success(orderIds);
        }
    }

    /// <summary>
    /// 綠界訂單創建(新增)
    /// </summary>
    /// <param name="orderId">多筆訂單 ID </param>
    /// <param name="userId">使用者 ID </param>
    /// <param name="tunnelUrl">開發者通道網址</param>
    /// <returns>跳轉綠界訂單</returns>
    public async Task<ApiResponse<GreenPayResponse>> GetPaymentData(List<int> orderId, int userId, string tunnelUrl)
    {
        // 所有訂單加總的金額
        decimal totalAmount = 0;
        string orderNumber = "";
        OrderResponse target = new OrderResponse();
        foreach (int Id in orderId)
        {
            target = await productsBuyRepositories.GetUserOneOrder(Id, userId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<GreenPayResponse>();
            }
            if (string.IsNullOrEmpty(orderNumber))
            {
                orderNumber = target.OrderNumber;
            }

            totalAmount += target.AccountAmount;
        }

        var ecpay = new Dictionary<string, string>
        {
            { "MerchantID", "3002607" }, //這是測試用的商店編號,固定的
            { "MerchantTradeNo", orderNumber },
            { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") }, //交易當下時間
            { "PaymentType", "aio" }, //付款類型=>全金流
            { "TotalAmount", Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero).ToString() },
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

            var orderNo = collection["MerchantTradeNo"].ToString();
            // 交易失敗的話
            if (rtnCode != "1")
            {
                var failedOrders = await logisticsRepository.GetByOrderNumber(orderNo);

                if (failedOrders != null)
                {
                    // 從這些 Order 裡取出 LogisticsId，去重（同賣家多商品會共用同一個 LogisticsId）
                    var logisticsIds = failedOrders.Select(o => o.LogisticsId).Distinct();

                    foreach (var logisticsId in logisticsIds)
                    {
                        await logisticsRepository.UpdateStatus(logisticsId, LogisticsStatusEnum.Cancelled);
                    }
                }
                return "0|PaymentFailed";
            }

            //訂單成立之後,開始更新資料庫
            var couponCompleted = await couponRepository.CompleteUserCoupon(orderNo);

            var buyProduct = await productsBuyRepositories.GetOrderByOrderNumber(orderNo);
            if (buyProduct == null)
            {
                return "0|OrderNotFound_CheckDB";
            }

            string tradeAmt = collection["TradeAmt"].ToString();

            if (!decimal.TryParse(tradeAmt, out decimal totalPrice))
                return "0|InvalidTradeAmt";

            var totalAmount = buyProduct.Sum(o => o.AccountAmount);

            if (totalPrice != totalAmount)
                //金額不符,可能是資料被竄改了,不處理這筆訂單
                return "0|InvalidAmount";

            DateTime.TryParse(collection["PaymentDate"], out DateTime paidTime);
            if (paidTime == DateTime.MinValue)
            {
                paidTime = DateTime.Now; // 如果解析不到時間，就用系統現在時間
            }

            using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var paymentCompleted = await productsBuyRepositories.PaidProducts(
                    orderNo,
                    (int)ShippingStatusEnum.PendingShipment,
                    collection["PaymentType"].ToString(),
                    paidTime
                );
                if (paymentCompleted <= 0)
                {
                    return "0|DBUpdateFailed";
                }
                // 交易成功後 , 更新物流單狀態
                var successOrders = await logisticsRepository.GetByOrderNumber(orderNo);
                if (successOrders != null)
                {
                    var logisticsIds = successOrders.Select(o => o.LogisticsId).Distinct();
                    foreach (var logisticsId in logisticsIds)
                    {
                        await logisticsRepository.UpdateStatus(logisticsId, LogisticsStatusEnum.PendingShipment);
                    }
                }

                trxScope.Complete();
            }
            // 開始呼叫綠界的物流 API , 生成物流單 ( 不寫在交易裡是因為交易只管系統內部 , 這種呼叫外部操作的他管不到 )
            var confirmedOrders = await logisticsRepository.GetByOrderNumber(orderNo);

            if (confirmedOrders != null)
            {
                var logisticsIds = confirmedOrders.Select(o => o.LogisticsId).Distinct();

                foreach (var logisticsId in logisticsIds)
                {
                    await logisticsRepository.UpdateStatus(logisticsId, LogisticsStatusEnum.PendingShipment);

                    // logistics 是這張物流單的完整資訊
                    var logistics = confirmedOrders.First(o => o.LogisticsId == logisticsId);

                    // ordersUnderThisLogistics 是這張物流單底下的所有訂單
                    var ordersUnderThisLogistics = buyProduct.Where(o => o.LogisticsId == logisticsId);

                    // 商品名稱可能不只一種，用逗號串起來，避免只顯示第一件
                    var productNames = ordersUnderThisLogistics.Select(o => o.ProductsName).Distinct().ToList();
                    var goodsName =
                        productNames.Count == 1 ? productNames[0] : $"{productNames[0]} 等{productNames.Count}件商品";
                    // 欄位長度限制
                    if (goodsName.Length > 20)
                    {
                        goodsName = goodsName.Substring(0, 20) + "...";
                    }

                    if (logistics.LogisticsType == "CVS")
                    {
                        // 這幾筆訂單的賣家是誰
                        var sellerId = ordersUnderThisLogistics.First().SellerUserId;
                        var seller = await userRepository.GetUser(sellerId);

                        var createInfo = new LogisticsOrderInfoRequest
                        {
                            MerchantTradeNo = logistics.MerchantTradeNo,
                            LogisticsSubType = logistics.LogisticsSubType,
                            GoodsAmount = ordersUnderThisLogistics.Sum(o => o.AccountAmount),
                            GoodsName = goodsName,
                            SenderName = seller?.UserName ?? "賣家",
                            SenderPhone = seller?.UserPhone,
                            ReceiverName = logistics.ReceiverName,
                            ReceiverPhone = logistics.ReceiverPhone,
                            ReceiverStoreID = logistics.StoreCode ?? "",
                        };

                        var createResult = await logisticsService.CreateLogisticsOrder(createInfo);

                        Console.WriteLine(
                            $"CodeStatus: {createResult.CodeStatus}, ReturnData: {string.Join(",", createResult.ReturnData?.Select(kv => $"{kv.Key}={kv.Value}") ?? [])}"
                        );
                        if (
                            createResult.CodeStatus == CodeStatusEnum.Success
                            && createResult.ReturnData!.TryGetValue("AllPayLogisticsID", out var trackingNo)
                        )
                        {
                            // 綠界成功的話就會回傳物流編號 , 再把它更新到資料庫
                            await logisticsRepository.UpdateTrackingNo(logisticsId, trackingNo);
                        }
                    }
                }
            }
            return "1|OK";
        }
        else
        {
            //驗證失敗..丟掉
            return "0|CheckMacValueVerifyFail";
        }
    }

    /// <summary>
    /// 綠界訂單創建( 重新付款 )
    /// </summary>
    /// <param name="orderIds">多筆訂單 ID </param>
    /// <param name="userId">使用者 ID </param>
    /// <param name="tunnelUrl">開發者通道網址</param>
    /// <returns>跳轉綠界訂單</returns>
    public async Task<ApiResponse<GreenPayResponse>> GetRetryPaymentData(
        List<int> orderIds,
        int userId,
        string tunnelUrl
    )
    {
        // 重新付款的話就重新生一個訂單編號
        string merchantTradeNo = "GN" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 11);

        // 所有訂單加總的金額
        decimal totalAmount = 0;
        foreach (int Id in orderIds)
        {
            var target = await productsBuyRepositories.GetUserOneOrder(Id, userId);

            if (target == null)
            {
                return ApiResponseHelper.NotFound<GreenPayResponse>();
            }

            if (target.ShippingStatus != (int)ShippingStatusEnum.PendingPayment)
            {
                var errors = new Dictionary<string, string[]> { { "ShippingStatus", new[] { "這筆訂單已付款完成!" } } };

                return ApiResponseHelper.RequestError<GreenPayResponse>(errors);
            }

            totalAmount += target.AccountAmount;
        }
        using (var trxScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            // 重新生成訂單編號
            await productsBuyRepositories.RetryPaidProducts(orderIds, merchantTradeNo);
            // 撈出這批 orderIds 底下，實際牽涉到哪幾張不同的物流單
            var logisticsIds = await logisticsRepository.GetLogisticsIdsByOrderIds(orderIds);
            // 重新生成物流訂單編號
            int shipmentIndex = 0;

            foreach (var logisticsId in logisticsIds)
            {
                shipmentIndex++;
                var no = $"{merchantTradeNo}-{shipmentIndex}";
                await logisticsRepository.UpdateMerchantTradeNo(logisticsId, no);
            }

            trxScope.Complete();
        }

        // 接下來就跟原本付款邏輯一樣
        var ecpay = new Dictionary<string, string>
        {
            { "MerchantID", "3002607" }, //這是測試用的商店編號,固定的
            { "MerchantTradeNo", merchantTradeNo },
            { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
            { "PaymentType", "aio" },
            { "TotalAmount", Math.Round(totalAmount, 0, MidpointRounding.AwayFromZero).ToString() },
            { "TradeDesc", "商品購買" },
            { "ItemName", "商品名稱" },
            { "ReturnURL", $"{tunnelUrl}/api/Order/EcPayBack" },
            { "OrderResultURL", $"{tunnelUrl}/api/Order/PaymentCallback" },
            { "ChoosePayment", "ALL" },
            { "EncryptType", "1" },
        };
        ecpay["CheckMacValue"] = ECPayHelper.GetCheckMacValue(ecpay);

        var result = new GreenPayResponse
        {
            FormData = ecpay,
            ActionUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5",
        };

        return ApiResponseHelper.Success(result);
    }
}
