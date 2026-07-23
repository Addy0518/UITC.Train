namespace Lab.Accounting.API.Infrastructures.Data.Entities
{
    public class Notification
    {
        /// <summary>
        /// 通知 ID
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// 通知要給誰看
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 通知類型
        /// </summary>
        public NotificationTypeEnum NotificationType { get; set; }

        /// <summary>
        /// 通知標題
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 通知內容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 相關聯的 ID ( 例如訂單 ID、審核表 ID，點擊通知時導頁用 )
        /// </summary>
        public int? RelatedId { get; set; }

        /// <summary>
        /// 是否已讀
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
