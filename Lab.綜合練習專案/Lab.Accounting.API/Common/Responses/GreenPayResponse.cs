namespace Lab.Accounting.API.Common.Responses
{
    public class GreenPayResponse
    {
        /// <summary>
        /// 傳給綠界的規定格式資料
        /// </summary>
        public Dictionary<string, string> FormData { get; set; }

        /// <summary>
        /// 綠界接收資料的網址
        /// </summary>
        public string ActionUrl { get; set; }
    }
}
