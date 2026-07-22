import accountApiInstance from '@/api/accountInstance.js';

// 賣場相關 API ===========================================================

// 查看賣場
export const getStore = (id) => accountApiInstance.get(`Store/GetStore?sellerId=${id}`);

// 賣場註冊
export const register = (userRegister) => accountApiInstance.post(`Store/Register`, userRegister);


// 編輯賣場
export const updateStore = (userRegister) =>
  accountApiInstance.put(`Store/UpdateStore`, userRegister);

// 編輯賣場
export const storeUpdateToCompany = (userRegister) =>
  accountApiInstance.put(`Store/StoreUpdateToCompany`, userRegister);
