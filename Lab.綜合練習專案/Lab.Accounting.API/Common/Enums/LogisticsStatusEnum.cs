namespace Lab.Accounting.API.Common.Enums;

public enum LogisticsStatusEnum
{
    [Description("物流單已建立")]
    Created = 0,

    [Description("賣家已出貨")]
    Shipped = 1,

    [Description("配送中")]
    InTransit = 2,

    [Description("已送達門市/已配達")]
    Delivered = 3,

    [Description("買家已取件")]
    PickedUp = 4,

    [Description("已取消")]
    Cancelled = 5,
}
