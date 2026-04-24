namespace Lab.Accounting.API.Common.Enums
{
    public enum ShippingStatus
    {
        [Description("待付款")]
        PendingPayment = 0,

        [Description("待出貨")]
        PendingShipment = 1,

        [Description("運送中")]
        InTransit = 2,

        [Description("已抵達")]
        Arrived = 3,
    }
}
