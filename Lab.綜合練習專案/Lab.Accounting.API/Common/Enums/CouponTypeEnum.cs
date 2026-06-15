namespace Lab.Accounting.API.Common.Enums;

public enum CouponTypeEnum
{
    [Description("百分比折扣")]
    百分比折扣 = 0,

    [Description("固定金額折抵")]
    固定金額折抵 = 1,

    [Description("免運券")]
    免運券 = 2,

    [Description("商品特價券")]
    商品特價券 = 3,
}
