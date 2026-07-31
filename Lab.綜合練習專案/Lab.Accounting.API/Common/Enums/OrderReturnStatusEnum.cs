namespace Lab.Accounting.API.Common.Enums;

// 狀態碼
public enum OrderReturnStatusEnum
{
    [Description("買家申請，待賣家審核")]
    Pending = 0,

    [Description(" 賣家同意退貨，等買家寄回")]
    Rejected = 2,

    [Description(" 買家填寫寄回追蹤編號，等賣家收貨")]
    Shipped = 3,

    [Description(" 賣家確認收到退貨")]
    Received = 4,

    [Description(" 已退款完成")]
    Refunded = 5,
}
