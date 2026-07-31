namespace Lab.Accounting.API.Common.Enums;

public enum ShippingStatusEnum
{
    [Description("待付款")]
    PendingPayment = 0,

    [Description("待出貨")]
    PendingShipment = 1,

    [Description("運送中")]
    InTransit = 2,

    [Description("已抵達門市")]
    Arrived = 3,

    [Description("已完成取貨")]
    Completed = 4,

    [Description("已取消")]
    Cancelled = 5,

    [Description("退貨處理中")]
    Returning = 6,
}
