namespace Lab.Accounting.API.Common.Enums;

// 狀態碼
public enum NotificationTypeEnum
{
    [Description(" 商品審核通過")]
    ProductApproved = 1,

    [Description(" 商品審核駁回")]
    ProductRejected = 2,

    [Description(" 企業賣場審核通過")]
    StoreCompanyApproved = 3,

    [Description(" 企業賣場審核駁回")]
    StoreCompanyRejected = 4,

    [Description(" 訂單物流狀態更新 ( 已出貨 / 已送達 / 買家已取貨 )")]
    LogisticsStatusUpdated = 5,

    [Description(" 賣家收到新訂單")]
    NewOrder = 6,

    [Description(" 賣家評論被回覆 ( 或評論收到賣家回覆，通知買家 )")]
    ProductRateReplied = 7,

    [Description(" 商品審核中")]
    ProductUnderReview = 8,
}
