namespace Lab.Accounting.API.Common.Responses
{
    public class RateResponse
    {
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 使用者大頭照
        /// </summary>
        public string? UserHeadshot { get; set; }

        /// <summary>
        /// 評分
        /// </summary>
        public double Rating { get; set; }

        /// <summary>
        /// 評論
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 發表時間
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
