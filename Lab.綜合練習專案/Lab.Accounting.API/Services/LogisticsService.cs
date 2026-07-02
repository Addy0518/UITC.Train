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
                { "MerchantTradeNo", request.MerchantTradeNo },
                { "LogisticsType", "CVS" },
                { "LogisticsSubType", request.LogisticsSubType },
                { "IsCollection", "N" },
                { "ServerReplyURL", $"{_settings.ServerBaseUrl}/api/Logistics/CvsStoreCallback" },
                { "ExtraData", request.MerchantTradeNo },
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
        /// 接收綠界回傳的門市資料存進暫存表
        /// </summary>
        /// <param name="request">綠界回傳門市資料</param>
        /// <returns></returns>
        public async Task HandleCvsStoreCallback(CvsStoreCallbackRequest request)
        {
            // ExtraData 帶的就是 SessionKey
            var sessionKey = request.ExtraData;
            if (string.IsNullOrEmpty(sessionKey))
                return;

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
            await logisticsTempRepository.CreateLogisticsTemp(temp);
        }

        /// <summary>
        /// 收件人資料存進暫存表
        /// </summary>
        /// <param name="request">物流暫存表單資料</param>
        /// <returns>操作結果</returns>
        public async Task<ApiResponse<string>> SaveLogisticsTemp(LogisticsTempInsertRequest request)
        {
            var existingTemp = await logisticsTempRepository.GetLogisticsTemp(request.SessionKey);

            if (existingTemp == null)
            {
                // 宅配 or 第一次存（超商應該在 CvsStoreCallback 已建好，走這裡是保護邏輯）
                var temp = new OrderLogisticsTemp
                {
                    SessionKey = request.SessionKey,
                    LogisticsType = request.LogisticsType,
                    LogisticsSubType = request.LogisticsSubType,
                    StoreCode = request.StoreCode,
                    StoreName = request.StoreName,
                    StoreAddress = request.StoreAddress,
                    ReceiverName = request.ReceiverName,
                    ReceiverPhone = request.ReceiverPhone,
                    ReceiverAddress = request.ReceiverAddress,
                    ExpiredAt = DateTime.Now.AddHours(2),
                };
                await logisticsTempRepository.CreateLogisticsTemp(temp);
            }
            else
            {
                await logisticsTempRepository.DeleteBySessionKey(request.SessionKey);

                // 超商：補上收件人姓名電話（門市資料已存在）
                existingTemp.ReceiverName = request.ReceiverName;
                existingTemp.ReceiverPhone = request.ReceiverPhone;
                existingTemp.ExpiredAt = DateTime.Now.AddHours(2); // 重新計算過期時間
                await logisticsTempRepository.CreateLogisticsTemp(existingTemp);
            }

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
