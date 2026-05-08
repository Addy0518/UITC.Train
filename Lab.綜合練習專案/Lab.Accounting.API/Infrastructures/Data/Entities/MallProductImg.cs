namespace Lab.Accounting.API.Infrastructures.Data.Entities;

public class MallProductImg
{
    /// <summary>
    /// 商品圖片 ID
    /// </summary>
    public int ProductsImgId { get; set; }

    /// <summary>
    /// 商品 ID
    /// </summary>
    public int ProductsId { get; set; }

    /// <summary>
    /// 商品圖片 URL
    /// </summary>
    public string ProductsImg { get; set; }
}
