using Lab.Accounting.API.Common.Requests.Logistics;
using Microsoft.Extensions.Options;

namespace Lab.Accounting.API.Services
{
    public class LogisticsService(
        ILogisticsTempRepository logisticsTempRepository,
        IOptions<EcpayLogisticsSettings> ecpayLogisticsOptions
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
        /// 呼叫綠界建立物流訂單 ( 超商 )
        /// </summary>
        /// <param name="request">物流訂單資訊</param>
        /// <returns>綠界回傳資料</returns>
        public async Task<ApiResponse<Dictionary<string, string>>> CreateLogisticsOrder(
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
                { "ServerReplyURL", $"{_settings.ServerBaseUrl}/api/Logistics/LogisticsStatusNotify" },
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

            using var httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(parameters);
            var response = await httpClient.PostAsync(LogisticsStageUrl, content);
            var resultBody = await response.Content.ReadAsStringAsync();

            var resultDict = HttpUtility
                .ParseQueryString(resultBody)
                .AllKeys.Where(k => k != null)
                .ToDictionary(k => k!, k => HttpUtility.ParseQueryString(resultBody)[k] ?? "");

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
            var homeTemp = new OrderLogisticsTemp
            {
                SessionKey = request.SessionKey,
                LogisticsType = request.LogisticsType,
                LogisticsSubType = request.LogisticsSubType,
                ReceiverName = request.ReceiverName,
                ReceiverPhone = request.ReceiverPhone,
                ReceiverAddress = request.ReceiverAddress,
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
    }
}
