import accountApiInstance from '@/api/accountInstance.js';

// 賣場相關 API ===========================================================

// 查看賣場
export const getSeller = () => accountApiInstance.post(`Seller/GetSeller`);

// 賣場註冊
export const register = (userRegister) => accountApiInstance.post(`Seller/Register`, userRegister);

// 編輯賣場
export const updateSeller = (userRegister) =>
  accountApiInstance.post(`Seller/UpdateSeller`, userRegister);
