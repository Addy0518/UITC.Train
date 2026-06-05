namespace Lab.Accounting.API.Common.Requests.Products;

public class ProductsUpdateRequest
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    [Display(Name = "商品 ID")]
    [Required(ErrorMessage = "{0} 必輸")]
    public int ProductsId { get; set; }

    /// <summary>
    /// 使用者 ID
    /// </summary>
    [Display(Name = "使用者ID")]
    [Required(ErrorMessage = "{0} 必輸")]
    public int UserId { get; set; }

    /// <summary>
    /// 商品類別 ID
    /// </summary>
    [Display(Name = "商品類別 ID")]
    public int ProductCategoryId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    [Display(Name = "商品名稱")]
    public string? ProductsName { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    [Display(Name = "商品價格")]
    public decimal? ProductsPrice { get; set; }

    /// <summary>
    /// 商品庫存數量
    /// </summary>
    [Display(Name = "商品庫存數量")]
    public int? ProductsStock { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    [Display(Name = "商品描述")]
    [MaxLength(500, ErrorMessage = "{0} 不可超過 {1} 字!")]
    [MinLength(1, ErrorMessage = "商品描述不可為空！")]
    public string? ProductsDescription { get; set; }
}
