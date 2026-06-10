namespace Lab.Accounting.API.Infrastructures.Data.Entities;

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
    /// 商品名稱
    /// </summary>
    public string ProductsName { get; set; }

    /// <summary>
    /// 商品價格
    /// </summary>
    public decimal? ProductsPrice { get; set; }

    /// <summary>
    /// 商品庫存數量
    /// </summary>
    public int? ProductsStock { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? ProductsDescription { get; set; }

    /// <summary>
    /// 商品類別 ID
    /// </summary>
    public int ProductCategoryId { get; set; }

    /// <summary>
    /// 審核狀態 (例如：1=待審核, 2=審核通過, 3=審核失敗)
    /// </summary>
    public ReviewStatusEnum ReviewStatus { get; set; }

    /// <summary>
    /// 創建時間
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 是否為刪除狀態
    /// </summary>
    public IsDeleteStatusEnum IsDelete { get; set; }
}
