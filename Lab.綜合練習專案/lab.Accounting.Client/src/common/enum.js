/**
 * 取得 Enum 的描述
 * @param {Object} enumObj enums.js 中的 enum 物件
 * @param {Number} value enum 的值
 * @returns
 */
export const getEnumDescription = (enumObj, value) => {
  const key = Object.keys(enumObj).find((key) => enumObj[key].value === value);
  if (key) return enumObj[key].description;
  else {
    console.error(`enum value '${value}' is undefined`);
    return value;
  }
};

// Http 回傳狀態碼
export const httpCodeStatusEnum = Object.freeze({
  Ok: 200,
  BadRequest: 400,
  Unauthorized: 401,
  Forbidden: 403,
  NotFound: 404,
  MethodNotAllowed: 405,
  InternalServerError: 500,
  ServiceUnavailable: 503,
});

// 自訂回傳狀態碼
export const codeStatusEnum = Object.freeze({
  // 成功
  Success: 2000,
  // Request驗證失敗
  RequestError: 4000,
  // 查無資料
  NotFound: 4001,
  // 內部伺服器錯誤
  InternalException: 5000,
});

// 刪除狀態
export const isDeleteEnum = Object.freeze({
  Normal: { value: 0, description: '正常' },
  Delete: { value: 1, description: '軟刪除' },
});

// 性別
export const genderEnum = Object.freeze({
  Man: { value: 0, description: '女性' },
  Woman: { value: 1, description: '男性' },
  Else: { value: 2, description: '其他' },
});

// 商品運送狀態
export const shippingEnum = Object.freeze({
  PendingPayment: { value: 0, description: '待付款' },
  PendingShipment: { value: 1, description: '待出貨' },
  InTransit: { value: 2, description: '運送中' },
  Arrived: { value: 3, description: '已抵達' },
});

// 審核狀態
export const reviewStatusEnum = Object.freeze({
  All: { value: null, description: '全部' },
  Pending: { value: 0, description: '待審核' },
  Approved: { value: 1, description: '審核通過' },
  Reject: { value: 2, description: '駁回申請' },
});
