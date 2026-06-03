namespace Lab.Accounting.API.Common.Enums;

// 狀態碼
public enum ReviewStatusEnum
{
    [Description("待審核")]
    Pending = 0,

    [Description("審核通過")]
    Approved = 1,

    [Description("駁回申請")]
    Reject = 2,
}
