import accountApiInstance from '@/api/accountInstance.js';

// 訂單相關 API ===========================================================

// 使用者購買商品
export const userBuyProduct = (buyRequest) =>
  accountApiInstance.post(`Order/UserBuyProduct`, buyRequest);
