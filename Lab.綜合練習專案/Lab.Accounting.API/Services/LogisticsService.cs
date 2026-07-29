using Lab.Accounting.API.Common.Requests.Logistics;
using Microsoft.Extensions.Options;

namespace Lab.Accounting.API.Services
{
    public class LogisticsService(
        ILogisticsTempRepository logisticsTempRepository,
        ILogisticsRepository logisticsRepository,
        IOptions<EcpayLogisticsSettings> ecpayLogisticsOptions,
        IProductsOrderRepository productsOrderRepository,
        INotificationService notificationService
    ) : ILogisticsService
    {
        private readonly EcpayLogisticsSettings _settings = ecpayLogisticsOptions.Value;
        private const string LogisticsStageUrl = "https://logistics-stage.ecpay.com.tw";

        /// <summary>
        /// 產生綠界超商門市地圖網址
        /// </summary>
        /// <param name="request">物流訂單資訊</param>
        /// <returns>地圖網址</returns>
        public ApiResponse<Dictionary<string, string>> GetCvsMapUrl(GetCvsMapRequest request)
        {
            // ExtraData 帶 SessionKey（也就是 MerchantTradeNo）
            // 選完門市綠界 POST 回來時，後端靠這個知道是哪一筆結帳的暫存資料要更新
            var parameters = new SortedDictionary<string, string>
            {
                { "MerchantID", _settings.MerchantId },
                { "MerchantTradeNo", request.SessionKey },
                { "LogisticsType", "CVS" },
                { "LogisticsSubType", request.LogisticsSubType },
                { "IsCollection", "N" },
                { "ServerReplyURL", $"{_settings.ServerBaseUrl}/api/Logistics/CvsStoreCallback" },
                { "ExtraData", request.SessionKey },
                { "Device", "0" },
            };

            // 用跟金流一樣的方式計算 CheckMacValue , 不過加密方式是 MD5
            var checkMacValue = ECPayHelper.GetCheckMacValueMD5(
                new Dictionary<string, string>(parameters),
                _settings.HashKey,
                _settings.HashIV
            );
            parameters.Add("CheckMacValue", checkMacValue);

            var result = new Dictionary<string, string>(parameters);
            result.Add("ActionUrl", $"{LogisticsStageUrl}/Express/map");

            return ApiResponseHelper.Success(result);
        }

        /// <summary>
        /// 接收綠界物流狀態通知，更新對應物流單的狀態
        /// </summary>
        /// <param name="request">綠界回傳的物流狀態資料</param>
        /// <returns>是否處理成功</returns>
        public async Task<bool> HandleLogisticsStatusNotify(LogisticsStatusCallbackRequest request)
        {
            var logistics = await logisticsRepository.GetByTrackingNo(request.AllPayLogisticsID);

            if (logistics == null)
            {
                return false;
            }

            var newStatus = MapRtnToStatus(request.LogisticsStatus);

            if (newStatus.HasValue)
            {
                await logisticsRepository.UpdateStatus(
                    logistics.LogisticsId,
                    newStatus.Value,
                    request.LogisticsStatus,
                    request.RtnMsg
                );

                var orderStatus = MapLogisticsStatusToOrderStatus(newStatus.Value);
                if (orderStatus.HasValue)
                {
                    await productsOrderRepository.UpdateShippingStatusByLogisticsId(
                        logistics.LogisticsId,
                        orderStatus.Value
                    );
                }
                // 給買家傳通知訊息
                await NotifyUser(logistics, newStatus.Value, request.RtnMsg);
            }
            return true;
        }

        /// <summary>
        /// 呼叫綠界建立物流訂單 ( 超商 )
        /// </summary>
        /// <param name="request">物流訂單資訊</param>
        /// <returns>綠界回傳資料</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> CreateCVSLogisticsOrder(
            LogisticsOrderInfoRequest request
        )
        {
            // 綠界規則 :　名字不能有空白，且長度不能超過 10 個字元
            string senderName = request.SenderName.Replace(" ", "");
            if (senderName.Length > 5)
                senderName = senderName.Substring(0, 5);

            string receiverName = request.ReceiverName.Replace(" ", "");
            if (receiverName.Length > 5)
                receiverName = receiverName.Substring(0, 5);

            // ExtraData 帶 SessionKey（也就是 MerchantTradeNo）
            // 選完門市綠界 POST 回來時，後端靠這個知道是哪一筆結帳的暫存資料要更新
            var parameters = new SortedDictionary<string, string>
            {
                { "MerchantID", _settings.MerchantId },
                { "MerchantTradeNo", request.MerchantTradeNo },
                { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                { "LogisticsType", "CVS" },
                { "LogisticsSubType", request.LogisticsSubType },
                { "GoodsAmount", ((int)request.GoodsAmount).ToString() },
                { "CollectionAmount", "0" }, // 沒有代收貨款（已經先線上刷卡付款完成）
                { "IsCollection", "N" },
                { "GoodsName", request.GoodsName },
                { "SenderName", senderName },
                { "SenderCellPhone", request.SenderCellPhone ?? request.SenderPhone ?? "" },
                { "ReceiverName", receiverName },
                { "ReceiverCellPhone", request.ReceiverCellPhone ?? request.ReceiverPhone ?? "" },
                { "ReceiverStoreID", request.ReceiverStoreID },
                { "ServerReplyURL", $"{_settings.ServerBaseUrl}/api/Logistics/HandleLogisticsStatusNotify" },
                // UNIMARTC2C 這個欄位不可為空，選店時發生問題（例如門市已關）綠界會透過這個網址通知
                { "LogisticsC2CReplyURL", $"{_settings.ServerBaseUrl}/api/Logistics/LogisticsC2CReply" },
            };

            // 用跟金流一樣的方式計算 CheckMacValue , 不過加密方式是 MD5
            var checkMacValue = ECPayHelper.GetCheckMacValueMD5(
                new Dictionary<string, string>(parameters),
                _settings.HashKey,
                _settings.HashIV
            );
            parameters.Add("CheckMacValue", checkMacValue);

            // 跟金流還有超商門市地圖不一樣 , 他們是前端傳送隱藏表單 form.submit() , 這裡是直接後端對後端
            // 所以直接用 http 請求工具發送 post 就行
            using var httpClient = new HttpClient();
            // FormUrlEncodedContent 是把 SortedDictionary<string, string> 轉成 application/x-www-form-urlencoded 的格式 ( 就是 key1=value1&key2=value2 這種 )
            var content = new FormUrlEncodedContent(parameters);
            // 發送 post 後用 response 接綠界發回的結果 , 再用 ReadAsStringAsync() 讀出來
            var response = await httpClient.PostAsync($"{LogisticsStageUrl}/Express/Create", content);
            var resultBody = await response.Content.ReadAsStringAsync();

            // 綠界固定格式為 "1|實際資料" 或 "0|錯誤訊息" , "1" 代表這次呼叫有被綠界成功接收
            var separatorIndex = resultBody.IndexOf('|');
            if (separatorIndex < 0 || resultBody.Substring(0, separatorIndex) != "1")
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "RtnCode", new[] { "建立物流訂單失敗 : " + resultBody } },
                };
                return ApiResponseHelper.RequestError<Dictionary<string, string>>(errors);
            }

            // 拿掉開頭的 "1|" , 剩下的才是真正的 querystring 資料
            var actualData = resultBody.Substring(separatorIndex + 1);

            // ParseQueryString 是把綠界回傳的結果 ( key1=value1&key2=value2 ) 轉成 NameValueCollection , 可以用 key 值去取值類似字典 , 但不是字典
            var parsed = HttpUtility.ParseQueryString(actualData);

            // 最後把這整理好的 NameValueCollection 轉成一般的字典 ( NameValueCollection 不能直接用 LINQ , 所以用 Allkeys 把裡面所有的 key（欄位名稱）撈出來變成一個字串陣列，這樣才能繼續往下用 LINQ)
            var resultDict = parsed.AllKeys.Where(k => k != null).ToDictionary(k => k!, k => parsed[k] ?? "");

            return ApiResponseHelper.Success(resultDict);
        }

        /// <summary>
        /// 呼叫綠界建立物流訂單 ( 宅配 )
        /// </summary>
        /// <param name="request">物流訂單資訊</param>
        /// <returns>綠界回傳資料</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> CreateHomeLogisticsOrder(
            LogisticsOrderInfoRequest request
        )
        {
            // 綠界規則 :　名字不能有空白，且長度不能超過 10 個字元
            string senderName = request.SenderName.Replace(" ", "");
            if (senderName.Length > 10)
                senderName = senderName.Substring(0, 10);

            string receiverName = request.ReceiverName.Replace(" ", "");
            if (receiverName.Length > 10)
                receiverName = receiverName.Substring(0, 10);

            // ExtraData 帶 SessionKey（也就是 MerchantTradeNo）
            // 選完門市綠界 POST 回來時，後端靠這個知道是哪一筆結帳的暫存資料要更新
            var parameters = new SortedDictionary<string, string>
            {
                { "MerchantID", _settings.MerchantId },
                { "MerchantTradeNo", request.MerchantTradeNo },
                { "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
                { "LogisticsType", "Home" },
                { "LogisticsSubType", request.LogisticsSubType },
                { "GoodsAmount", ((int)request.GoodsAmount).ToString() },
                { "CollectionAmount", "0" }, // 沒有代收貨款（已經先線上刷卡付款完成）
                { "IsCollection", "N" },
                { "GoodsName", request.GoodsName },
                { "SenderName", senderName },
                { "SenderCellPhone", request.SenderCellPhone ?? request.SenderPhone ?? "" },
                { "SenderZipCode", request.SenderZipCode ?? "" },
                { "SenderAddress", request.SenderAddress ?? "" },
                { "ReceiverName", receiverName },
                { "ReceiverCellPhone", request.ReceiverCellPhone ?? request.ReceiverPhone ?? "" },
                { "ReceiverAddress", request.ReceiverAddress ?? "" },
                { "ReceiverZipCode", request.ReceiverZipCode ?? "" },
                { "ServerReplyURL", $"{_settings.ServerBaseUrl}/api/Logistics/HandleLogisticsStatusNotify" },
            };

            // 用跟金流一樣的方式計算 CheckMacValue , 不過加密方式是 MD5
            var checkMacValue = ECPayHelper.GetCheckMacValueMD5(
                new Dictionary<string, string>(parameters),
                _settings.HashKey,
                _settings.HashIV
            );
            parameters.Add("CheckMacValue", checkMacValue);

            Console.WriteLine(
                $"[Home] SenderAddress: {parameters["SenderAddress"]}, ReceiverAddress: {parameters["ReceiverAddress"]}"
            );

            // 跟超商建立訂單邏輯一樣 , 要看解釋去超商那個 api 看
            using var httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync($"{LogisticsStageUrl}/Express/Create", content);
            var resultBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[Home] resultBody: {resultBody}");

            var separatorIndex = resultBody.IndexOf('|');
            if (separatorIndex < 0 || resultBody.Substring(0, separatorIndex) != "1")
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "RtnCode", new[] { "建立物流訂單失敗 : " + resultBody } },
                };
                return ApiResponseHelper.RequestError<Dictionary<string, string>>(errors);
            }

            var actualData = resultBody.Substring(separatorIndex + 1);
            var parsed = HttpUtility.ParseQueryString(actualData);
            var resultDict = parsed.AllKeys.Where(k => k != null).ToDictionary(k => k!, k => parsed[k] ?? "");

            return ApiResponseHelper.Success(resultDict);
        }

        /// <summary>
        /// 儲存物流暫存訂單資料 ( 超商 )
        /// </summary>
        /// <param name="request">綠界回傳門市資料</param>
        /// <returns>是否成功</returns>
        public async Task<ApiResponse<string>> SaveCvsLogisticsTemp(CvsStoreCallbackRequest request)
        {
            // ExtraData 帶的就是 SessionKey
            var sessionKey = request.ExtraData;
            if (string.IsNullOrEmpty(sessionKey))
                return ApiResponseHelper.NotFound<string>();

            var existingTemp = await logisticsTempRepository.GetLogisticsTemp(sessionKey);
            if (existingTemp != null)
            {
                // 買家重新選門市，先刪掉舊的
                await logisticsTempRepository.DeleteBySessionKey(sessionKey);
            }

            var temp = new OrderLogisticsTemp
            {
                SessionKey = sessionKey,
                LogisticsType = "CVS",
                LogisticsSubType = request.LogisticsSubType,
                StoreCode = request.CVSStoreID,
                StoreName = request.CVSStoreName,
                StoreAddress = request.CVSAddress,
                ReceiverName = string.Empty,
                ReceiverPhone = string.Empty,
                ExpiredAt = DateTime.Now.AddHours(2),
            };
            await logisticsTempRepository.CreateCVSLogisticsTemp(temp);

            return ApiResponseHelper.Success<string>("配送資訊已儲存");
        }

        /// <summary>
        /// 儲存物流暫存訂單收件人 ( 超商 )
        /// </summary>
        /// <param name="request">收件人資訊</param>
        /// <returns>是否成功</returns>
        public async Task<ApiResponse<string>> SaveCvsReceiver(CvsReceiverInsertRequest request)
        {
            var existingTemp = await logisticsTempRepository.GetLogisticsTemp(request.SessionKey);

            if (existingTemp == null)
            {
                var errors = new Dictionary<string, string[]>
                {
                    { "SessionKey", new[] { "找不到門市資料，請重新選擇門市" } },
                };
                return ApiResponseHelper.RequestError<string>(errors);
            }

            // 門市資料維持不動，只補姓名電話
            existingTemp.ReceiverName = request.ReceiverName;
            existingTemp.ReceiverPhone = request.ReceiverPhone;
            existingTemp.ExpiredAt = DateTime.Now.AddHours(2);

            await logisticsTempRepository.UpdateCVSLogisticsTemp(existingTemp);

            return ApiResponseHelper.Success<string>("收件人資訊已儲存");
        }

        /// <summary>
        /// 儲存物流暫存訂單資料 ( 宅配 )
        /// </summary>
        /// <param name="request">物流暫存表單資料</param>
        /// <returns>是否成功</returns>
        public async Task<ApiResponse<string>> SaveHomeLogisticsTemp(LogisticsTempInsertRequest request)
        {
            if (request.ReceiverAddress == null)
            {
                var errors = new Dictionary<string, string[]> { { "ReceiverAddress", new[] { "宅配地址不能為空" } } };
                return ApiResponseHelper.RequestError<string>(errors);
            }
            var homeTemp = new OrderLogisticsTemp
            {
                SessionKey = request.SessionKey,
                LogisticsType = request.LogisticsType,
                LogisticsSubType = request.LogisticsSubType,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                ReceiverAddress = request.ReceiverAddress,
                ReceiverZipCode = request.ReceiverZipCode,
                ExpiredAt = DateTime.Now.AddHours(2),
            };
            await logisticsTempRepository.CreateHomeLogisticsTemp(homeTemp);
            return ApiResponseHelper.Success<string>("配送資訊已儲存");
        }

        /// <summary>
        /// 查看物流暫存訂單資料
        /// </summary>
        /// <param name="sessionKey">SessionKey ( 對應金流的 MerchantTradeNo )</param>
        /// <returns>物流暫存訂單資料</returns>
        public async Task<ApiResponse<OrderLogisticsTemp>> GetLogisticsTemp(string sessionKey)
        {
            var temp = await logisticsTempRepository.GetLogisticsTemp(sessionKey);
            if (temp == null)
            {
                return ApiResponseHelper.NotFound<OrderLogisticsTemp>();
            }
            return ApiResponseHelper.Success(temp);
        }

        /// <summary>
        /// 映射綠界回傳的物流狀態碼到 LogisticsStatusEnum
        /// </summary>
        /// <param name="rtnCode">物流回傳碼</param>
        /// <returns>物流狀態</returns>
        private LogisticsStatusEnum? MapRtnToStatus(string rtnCode)
        {
            // 綠界回傳的所有狀態碼跟我的列舉直的對照表 ( 格式為 => C2C 店到店 , 統一超商門市 ( 7-11 ) )
            return rtnCode switch
            {
                // 訂單剛建立、處理中
                "300" => LogisticsStatusEnum.PendingShipment,

                // 賣家已出貨、貨物進入物流中心
                "2030" => LogisticsStatusEnum.Shipped, // 物流中心驗收成功
                "2068" => LogisticsStatusEnum.Shipped, // 賣家已到門市寄件

                // 配送中（貨物在途，但還沒到取貨門市）
                "2041" => LogisticsStatusEnum.InTransit, // 物流中心理貨中
                "2043" => LogisticsStatusEnum.InTransit, // 門市指定時間不配送，後續配送中
                "2058" => LogisticsStatusEnum.InTransit, // 天候不佳，後續配送中
                "2062" => LogisticsStatusEnum.InTransit, // 包裹門市確認中
                "2089" => LogisticsStatusEnum.InTransit, // 門市指定不配送(六、日)
                "2093" => LogisticsStatusEnum.InTransit, // 爆量
                "2102" => LogisticsStatusEnum.InTransit, // 門市舊店號更新
                "2105" => LogisticsStatusEnum.InTransit, // 已申請門市變更

                // 已送達取貨門市，等買家去拿
                "2073" => LogisticsStatusEnum.Delivered, // 包裹已配達取件門市
                "2098" => LogisticsStatusEnum.Delivered, // 包裹重新配達取件門市

                // 買家已經去門市取貨完成
                "2067" => LogisticsStatusEnum.PickedUp,

                // 走取消/退貨流程
                "2051" => LogisticsStatusEnum.Cancelled, // 賣家要求提早退貨
                "2069" => LogisticsStatusEnum.Cancelled, // 退貨便收件
                "2070" => LogisticsStatusEnum.Cancelled, // 賣家已取退回包裹
                "2072" => LogisticsStatusEnum.Cancelled, // 包裹已退至原寄件門市
                "2076" => LogisticsStatusEnum.Cancelled, // 買家未取包裹，已退回物流中心
                "2078" => LogisticsStatusEnum.Cancelled, //
                "2079" => LogisticsStatusEnum.Cancelled,
                "2080" => LogisticsStatusEnum.Cancelled,
                "2081" => LogisticsStatusEnum.Cancelled,
                "2082" => LogisticsStatusEnum.Cancelled,
                "2083" => LogisticsStatusEnum.Cancelled,
                "2084" => LogisticsStatusEnum.Cancelled,
                "2085" => LogisticsStatusEnum.Cancelled,
                "2086" => LogisticsStatusEnum.Cancelled,
                "2087" => LogisticsStatusEnum.Cancelled,
                "2088" => LogisticsStatusEnum.Cancelled,
                "2097" => LogisticsStatusEnum.Cancelled, // 包裹宅配退回中
                "2099" => LogisticsStatusEnum.Cancelled, // 包裹重新配達寄件門市
                "9999" => LogisticsStatusEnum.Cancelled, // 訂單取消

                // 異常，需人工處理（賣家/客服要介入）
                "2042" => LogisticsStatusEnum.Exception, // 包裹遺失，進入賠償程序
                "2048" => LogisticsStatusEnum.Exception, // 包裝異常
                "2053" => LogisticsStatusEnum.Exception, // 門市誤刷取件
                "2061" => LogisticsStatusEnum.Exception, // 包裹異常
                "2066" => LogisticsStatusEnum.Exception, // 包裹確認中，將退回物流中心
                "2074" => LogisticsStatusEnum.Exception, // 買家未取包裹，將退回物流中心
                "2075" => LogisticsStatusEnum.Exception, // 賣家未取包裹，將退回物流中心
                "2077" => LogisticsStatusEnum.Exception, // 賣家未取包裹，待申請退回
                "2092" => LogisticsStatusEnum.Exception, // 門市關轉（需重選門市）
                "2094" => LogisticsStatusEnum.Exception, // 包裹異常
                "2096" => LogisticsStatusEnum.Exception, // 賣家未取包裹，待申請退回
                "2101" => LogisticsStatusEnum.Exception, // 門市關轉店
                "2103" => LogisticsStatusEnum.Exception, // 無取件門市資料
                "2104" => LogisticsStatusEnum.Exception, // 門市關轉，請重選門市
                "2106" => LogisticsStatusEnum.Exception, // 重複寄件，需申請退回
                "7013" => LogisticsStatusEnum.Exception, // 訂單超過驗收期限(賣家未出貨)
                "7017" => LogisticsStatusEnum.Exception, // 取件包裹異常，協尋中
                "7018" => LogisticsStatusEnum.Exception, // 包裹遺失，進入賠償程序
                "7019" => LogisticsStatusEnum.Exception, // 寄件包裹異常，協尋中
                "7020" => LogisticsStatusEnum.Exception, // 包裹遺失，進入賠償程序
                "7038" => LogisticsStatusEnum.Exception, // 門市驗收異常

                _ => null, // 對不到的代碼，先不更新狀態
            };
        }

        /// <summary>
        /// 映射 LogisticsStatusEnum 到 ShippingStatusEnum
        /// </summary>
        /// <param name="logisticsStatus">物流單狀態列舉值</param>
        /// <returns>物流狀態</returns>
        private ShippingStatusEnum? MapLogisticsStatusToOrderStatus(LogisticsStatusEnum logisticsStatus)
        {
            return logisticsStatus switch
            {
                LogisticsStatusEnum.Shipped => ShippingStatusEnum.InTransit,
                LogisticsStatusEnum.InTransit => ShippingStatusEnum.InTransit,
                LogisticsStatusEnum.Delivered => ShippingStatusEnum.Arrived,
                LogisticsStatusEnum.PickedUp => ShippingStatusEnum.Completed,
                LogisticsStatusEnum.Cancelled => ShippingStatusEnum.Cancelled,
                LogisticsStatusEnum.Exception => null, // 異常先不動 Order 狀態，只記錄詳細訊息讓客服介入
                _ => null, // Created / PendingShipment 不需要觸發訂單狀態變化
            };
        }

        /// <summary>
        /// 依物流狀態，判斷是否需要通知買家並發送
        /// </summary>
        /// <param name="logistics">物流單資訊</param>
        /// <param name="status">最新物流狀態</param>
        /// <param name="rtnMsg">綠界回傳訊息</param>
        private async Task NotifyUser(OrderLogistics logistics, LogisticsStatusEnum status, string rtnMsg)
        {
            // 一張物流單底下可能有多筆訂單（同賣家多商品），但都是同一個買家，取第一筆的 UserId 即可
            var orders = await productsOrderRepository.GetOrdersByLogisticsId(logistics.LogisticsId);
            var buyerId = orders?.FirstOrDefault()?.UserId;
            if (buyerId == null)
                return;

            (NotificationTypeEnum type, string title, string content)? notify = status switch
            {
                LogisticsStatusEnum.Shipped => (
                    NotificationTypeEnum.LogisticsStatusUpdated,
                    "商品已出貨",
                    "您的訂單已由賣家出貨，正在配送途中。"
                ),
                LogisticsStatusEnum.Delivered => (
                    NotificationTypeEnum.LogisticsStatusUpdated,
                    "包裹已送達門市",
                    "您的訂單已送達指定門市，請盡快前往取貨。"
                ),
                LogisticsStatusEnum.Cancelled => (
                    NotificationTypeEnum.LogisticsStatusUpdated,
                    "訂單物流已取消",
                    "您的訂單物流狀態已被取消，如有疑問請聯繫客服。"
                ),
                LogisticsStatusEnum.Exception => (
                    NotificationTypeEnum.LogisticsStatusUpdated,
                    "訂單物流異常",
                    $"您的訂單物流發生異常：{rtnMsg}，我們正在協助處理。"
                ),
                _ => null,
            };

            if (notify.HasValue)
            {
                await notificationService.CreateNotification(
                    buyerId.Value,
                    notify.Value.type,
                    notify.Value.title,
                    notify.Value.content,
                    orders.FirstOrDefault().OrderId
                );
            }
        }

        // 測試綠界呼叫用 , 可以刪
        public async Task<string> GetCheckMacValueForTest(Dictionary<string, string> parameters)
        {
            return ECPayHelper.GetCheckMacValueMD5(parameters, _settings.HashKey, _settings.HashIV);
        }
    }
}
