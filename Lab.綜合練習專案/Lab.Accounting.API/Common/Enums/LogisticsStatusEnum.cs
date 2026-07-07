namespace Lab.Accounting.API.Common.Enums;

public enum LogisticsStatusEnum
{
    [Description("物流單已建立")]
    Created = 0,

    [Description("付款完成，等待出貨")]
    PendingShipment = 1,

    [Description("賣家已出貨")]
    Shipped = 2,

    [Description("配送中")]
    InTransit = 3,

    [Description("已送達門市/已配達")]
    Delivered = 4,

    [Description("買家已取件")]
    PickedUp = 5,

    [Description("已取消")]
    Cancelled = 6,

    [Description("異常，需人工處理")]
    Exception = 7,
}
