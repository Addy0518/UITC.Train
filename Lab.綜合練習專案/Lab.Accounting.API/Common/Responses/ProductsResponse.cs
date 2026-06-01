namespace Lab.Accounting.API.Common.Responses;

public class ProductsResponse
{
    public IEnumerable<Product> Products { get; set; }

    /// <summary>
    /// 總筆數
    /// </summary>
    public int? TotalCount { get; set; }
}

public class Product
{
    /// <summary>
    /// 商品 ID
    /// </summary>
    public int ProductsId { get; set; }

    /// <summary>
    /// 使用者 ID
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 商品類別 ID
    /// </summary>
    public int ProductCategoryId { get; set; }

    /// <summary>
    /// 商品類別名稱
    /// </summary>
    public string ProductCategoryName { get; set; }

    /// <summary>
    /// 父類別 ID ( 可為 null，表示該類別為頂層類別 )
    /// </summary>
    public int? ProductParentId { get; set; }

    /// <summary>
    /// 父類別名稱
    /// </summary>
    public string ParentCategoryName { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductsName { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    public decimal ProductsPrice { get; set; }

    /// <summary>
    /// 商品庫存數量
    /// </summary>
    public int ProductsStock { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? ProductsDescription { get; set; }

    /// <summary>
    /// 購買數量
    /// </summary>
    public int BoughtQuantity { get; set; }

    /// <summary>
    /// 商品總平均評分
    /// </summary>
    public decimal ProductsAVGRate { get; set; }

    /// <summary>
    /// 商品所有評價
    /// </summary>
    public IEnumerable<RateResponse>? ProductsAllRates { get; set; }

    /// <summary>
    /// 是否為刪除狀態
    /// </summary>
    public IsDeleteStatusEnum IsDelete { get; set; }

    /// <summary>
    /// 商品圖片 URL
    /// </summary>
    public IEnumerable<MallProductImg>? ProductsImgs { get; set; }

    /// <summary>
    /// 總筆數
    /// </summary>
    public int? TotalCount { get; set; }
}
