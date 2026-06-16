namespace Lab.Accounting.API.Common.Requests.Coupon
{
    public class UserCouponInsertRequest
    {
        /// <summary>
        /// 使用者 ID
        /// </summary>
        [Display(Name = "使用者 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int UserId { get; set; }

        /// <summary>
        /// 優惠券 ID
        /// </summary>
        [Display(Name = "優惠券 ID")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public int CouponId { get; set; }

        /// <summary>
        /// 領取時間
        /// </summary>
        [Display(Name = "領取時間")]
        [Required(ErrorMessage = "{0} 不能為空!")]
        public DateTime CreateTime { get; set; }
    }
}
