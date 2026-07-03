namespace Lab.Accounting.API.Common.Requests.Products;

public class ProductsBuyRequest
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    [Display(Name = "使用者 ID ")]
    [Required(ErrorMessage = "{0} 必輸")]
    public int UserId { get; set; }

    /// <summary>
    /// 物流訂單編號
    /// </summary>
    [Display(Name = "物流訂單編號")]
    [Required(ErrorMessage = "{0} 不能為空!")]
    [MaxLength(100, ErrorMessage = "{0} 長度最長為 {1} 字")]
    public string SessionKey { get; set; } = string.Empty;

    /// <summary>
    /// 所有購買的商品
    /// </summary>
    [Display(Name = "所有購買的商品")]
    [Required(ErrorMessage = "{0} 必輸")]
    public IEnumerable<ProductsItem> Products { get; set; }

    /// <summary>
    /// 優惠券 ID
    /// </summary>
    [Display(Name = "優惠券 ID")]
    public int? CouponId { get; set; }

    /// <summary>
    /// 購買時間
    /// </summary>
    [Display(Name = "購買時間")]
    [Required(ErrorMessage = "{0} 必輸")]
    public DateTime BoughtTime { get; set; }
}

public class ProductsItem
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    [Display(Name = "商品 ID")]
    [Required(ErrorMessage = "{0} 必輸")]
    public int ProductsId { get; set; }

    /// <summary>
    /// 購買數量
    /// </summary>
    [Display(Name = "購買數量")]
    [Required(ErrorMessage = "{0} 必輸")]
    [Range(1, double.MaxValue, ErrorMessage = "{0} 必須大於或等於 {1}!")]
    public int BoughtQuantity { get; set; }
}
