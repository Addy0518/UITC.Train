import accountApiInstance from '@/api/accountInstance.js';

// 訂單相關 API ===========================================================

// 使用者購買商品
export const userBuyProduct = (buyRequest) =>
  accountApiInstance.post(`Order/UserBuyProduct`, buyRequest);

// 買家查看所有訂單
export const getUserOrder = () => accountApiInstance.get(`Order/GetUserOrder`);

// 買家查看單一訂單
export const getUserOneOrder = (id) =>
  accountApiInstance.get(`Order/GetUserOneOrder?orderId=${id}`);

// 更新運輸狀態
export const updateShippingStatus = (id, status) =>
  accountApiInstance.put(`Order/UpdateShippingStatus?orderId=${id}&shippingStatus=${status}`);

// 賣家查看所有訂單
export const getSellerOrder = () => accountApiInstance.get(`Order/GetSellerOrder`);

// 賣家查看單一訂單
export const getSellerOneOrder = (id) =>
  accountApiInstance.get(`Order/GetSellerOneOrder?orderId=${id}`);
