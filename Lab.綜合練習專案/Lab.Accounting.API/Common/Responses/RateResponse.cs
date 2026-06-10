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

        /// <summary>
        /// 評價分布數量
        /// </summary>
        public int? RateDistribution { get; set; }
    }

    public class DashBoardRateResponse
    {
        /// <summary>
        /// 評分分布區域
        /// </summary>
        public int RateDistribution { get; set; }

        /// <summary>
        /// 評分分布數量
        /// </summary>
        public int RateCount { get; set; }
    }
}
