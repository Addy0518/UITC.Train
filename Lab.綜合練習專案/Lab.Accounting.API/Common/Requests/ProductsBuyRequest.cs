namespace Lab.Accounting.API.Common.Requests;

public class ProductsBuyRequest
{
    /// <summary>
    /// 使用者 ID
    /// </summary>
    [Display(Name = "使用者 ID ")]
    [Required(ErrorMessage = "{0} 必輸")]
    public int UserId { get; set; }

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
    public int BoughtQuantity { get; set; }

    /// <summary>
    /// 購買時間
    /// </summary>
    [Display(Name = "購買時間")]
    [Required(ErrorMessage = "{0} 必輸")]
    public DateTime BoughtTime { get; set; }

    /// <summary>
    /// 運送地址
    /// </summary>
    [Display(Name = "運送地址")]
    [Required(ErrorMessage = "{0} 必輸")]
    public string ShippingAddress { get; set; }
}
