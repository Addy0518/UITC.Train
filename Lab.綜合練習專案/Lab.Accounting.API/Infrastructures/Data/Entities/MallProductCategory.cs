namespace Lab.Accounting.API.Infrastructures.Data.Entities;

public class MallProductCategory
{
    /// <summary>
    /// 商品類別 ID
    /// </summary>
    public int ProductCategoryId { get; set; }

    /// <summary>
    /// 商品類別名稱
    /// </summary>
    public string ProductCategoryName { get; set; }

    /// <summary>
    /// 類別父 ID ( 可為 null，表示該類別為頂層類別 )
    /// </summary>
    public int? ProductParentId { get; set; }
}
